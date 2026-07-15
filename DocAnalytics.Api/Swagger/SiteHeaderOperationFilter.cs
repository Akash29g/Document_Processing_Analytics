using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi;                    // ⚠️ 2.x: namespace COLLAPSED (was Microsoft.OpenApi.Models)
using Swashbuckle.AspNetCore.SwaggerGen;


namespace DocAnalytics.Api.Swagger;

[ExcludeFromCodeCoverage]
// Adds an optional "X-Site-Id" header box to EVERY endpoint in Swagger UI,
// so tenant-scoped requests can pass the site the middleware looks for.
public sealed class SiteHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // ⚠️ 2.x: collections are no longer auto-initialised — guard against null
        operation.Parameters ??= new List<IOpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "X-Site-Id",
            In = ParameterLocation.Header,
            Required = false,
            Description = "Site to scope this request to (tenant isolation). Paste your site_id GUID.",
            Schema = new OpenApiSchema { Type = JsonSchemaType.String }  // ⚠️ 2.x: Type is an ENUM, not "string"
        });
    }
}
