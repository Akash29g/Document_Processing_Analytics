using System.Text.Json;
using System.Text.RegularExpressions;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using DocAnalytics.Service.Aws;
using Microsoft.Extensions.Options;

namespace DocAnalytics.Service.Extraction;

/// <summary>Default <see cref="IInvoiceExtractor"/> implementation using Amazon Bedrock (Nova) to extract invoice data from PDFs.</summary>
public sealed class NovaInvoiceExtractor : IInvoiceExtractor
{
    private readonly IAmazonBedrockRuntime _bedrock;
    private readonly AwsOptions _opts;

    private const string Prompt =
    "You are an invoice data extractor. Return ONLY valid JSON (no markdown, no ```).\n" +
    "Definitions:\n" +
    "- seller = the business ISSUING the invoice (the NAME, e.g. the letterhead brand).\n" +
    "- client = the recipient being billed ('Bill To' / 'Buyer' / 'Customer').\n" +
    "- invoice_number = the value after 'Invoice', '#', or 'Invoice No'. Null if truly absent.\n" +
    "- invoice_date = ISO format yyyy-MM-dd if possible.\n" +
    "- currency = REQUIRED, never null. ISO code inferred from the symbol/locale " +
"('$' -> \"USD\", '₹' -> \"INR\", '€' -> \"EUR\", '£' -> \"GBP\"). If no symbol is visible, use \"USD\".\n" +
    "- line_items = each product row: description, quantity, unit_price, line_total, category.\n" +
"- category = choose EXACTLY ONE of: \"Technology\", \"Furniture\", \"Office Supplies\", " +
"\"Services\", \"Other\". Only if none of these fit at all, invent a short 1-2 word category. " +
"Never leave it empty.\n" +
    "- subtotal = sum of line item amounts BEFORE discount/tax/shipping.\n" +
    "- discount = discount amount (positive number), 0 if none.\n" +
    "- tax = tax amount, 0 if none.\n" +
    "- shipping = shipping/freight amount, 0 if none.\n" +
    "- total = FINAL payable grand total (subtotal - discount + tax + shipping). Number only, no symbol.\n" +
    "All money values are plain numbers (no currency symbols, no commas).\n" +
    "Shape:\n" +
    "{ \"invoice_number\": null, \"invoice_date\": \"\", \"seller\": \"\", \"client\": \"\", " +
    "\"currency\": \"\", \"subtotal\": 0, \"discount\": 0, \"tax\": 0, \"shipping\": 0, \"total\": 0, " +
   "\"line_items\": [ { \"description\": \"\", \"quantity\": 0, \"unit_price\": 0, \"line_total\": 0, \"category\": \"\" } ] }";


    public NovaInvoiceExtractor(IAmazonBedrockRuntime bedrock, IOptions<AwsOptions> opts)
    {
        _bedrock = bedrock;
        _opts = opts.Value;
    }

    /// <inheritdoc />
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
                    GetDecimal(el, "line_total"),
                    GetString(el, "category")));
        }

        return new InvoiceExtractionResult(
            GetString(root, "invoice_number"),
            GetString(root, "invoice_date"),
            GetString(root, "seller"),
            GetString(root, "client"),
            GetString(root, "currency"),
            GetDecimal(root, "subtotal"),
            GetDecimal(root, "discount"),
            GetDecimal(root, "tax"),
            GetDecimal(root, "shipping"),
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
