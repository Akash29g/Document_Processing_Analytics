using System.Text.Json;
using System.Text.RegularExpressions;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using DocAnalytics.Service.Aws;
using Microsoft.Extensions.Options;

namespace DocAnalytics.Service.Extraction;

public sealed class NovaInvoiceExtractor : IInvoiceExtractor
{
    private readonly IAmazonBedrockRuntime _bedrock;
    private readonly AwsOptions _opts;

    private const string Prompt =
        "You are an invoice data extractor. Return ONLY valid JSON (no markdown, no ```).\n" +
        "Definitions:\n" +
        "- seller = the business ISSUING the invoice (the NAME, never the GSTIN/tax number).\n" +
        "- client = the recipient being billed (look for 'Buyer', 'Bill To', 'Customer').\n" +
        "- invoice_number = value labeled 'Invoice No'. If none exists, use null (NOT a GSTIN/PO).\n" +
        "- total = final payable amount as a number only (no currency symbol).\n" +
        "- line_items = each row: description, quantity, unit_price, line_total (numbers, null if absent).\n" +
        "Shape:\n" +
        "{ \"invoice_number\": null, \"invoice_date\": \"\", \"seller\": \"\", \"client\": \"\", " +
        "\"total\": 0, \"line_items\": [ { \"description\": \"\", \"quantity\": 0, " +
        "\"unit_price\": 0, \"line_total\": 0 } ] }";

    public NovaInvoiceExtractor(IAmazonBedrockRuntime bedrock, IOptions<AwsOptions> opts)
    {
        _bedrock = bedrock;
        _opts = opts.Value;
    }

    public async Task<InvoiceExtractionResult> ExtractAsync(byte[] pdfBytes, CancellationToken ct = default)
    {
        var request = new ConverseRequest
        {
            ModelId = _opts.NovaModelId,
            Messages = new List<Message>
            {
                new()
                {
                    Role = ConversationRole.User,
                    Content = new List<ContentBlock>
                    {
                        new() { Text = Prompt },
                        new()
                        {
                            Document = new DocumentBlock
                            {
                                Format = DocumentFormat.Pdf,
                                Name   = "invoice",
                                Source = new DocumentSource { Bytes = new MemoryStream(pdfBytes) }
                            }
                        }
                    }
                }
            }
        };

        var resp = await _bedrock.ConverseAsync(request, ct);
        var raw = resp.Output.Message.Content[0].Text ?? "";
        return Parse(raw);
    }

    // strip ```json fences, then deserialize
    private static InvoiceExtractionResult Parse(string raw)
    {
        var json = Regex.Replace(raw.Trim(), @"^```(?:json)?|```$", "",
                                 RegexOptions.Multiline).Trim();

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var items = new List<ExtractedLineItem>();
        if (root.TryGetProperty("line_items", out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            int n = 1;
            foreach (var el in arr.EnumerateArray())
                items.Add(new ExtractedLineItem(
                    n++,
                    GetString(el, "description") ?? $"Item {n}",
                    GetDecimal(el, "quantity"),
                    GetDecimal(el, "unit_price"),
                    GetDecimal(el, "line_total")));
        }

        return new InvoiceExtractionResult(
            GetString(root, "invoice_number"),
            GetString(root, "invoice_date"),
            GetString(root, "seller"),
            GetString(root, "client"),
            GetDecimal(root, "total"),
            items);
    }

    private static string? GetString(JsonElement e, string p) =>
        e.TryGetProperty(p, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static decimal? GetDecimal(JsonElement e, string p)
    {
        if (!e.TryGetProperty(p, out var v)) return null;
        if (v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out var d)) return d;
        if (v.ValueKind == JsonValueKind.String && decimal.TryParse(v.GetString(), out var s)) return s;
        return null;
    }
}
