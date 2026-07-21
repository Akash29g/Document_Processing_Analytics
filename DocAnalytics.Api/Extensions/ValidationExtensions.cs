using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using DocAnalytics.Api.Common;
using Microsoft.AspNetCore.Mvc;


namespace DocAnalytics.Api.Extensions;

/// <summary>Overrides the framework's default model-validation 400 response with the standard <see cref="ApiResponse{T}"/> failure envelope.</summary>
[ExcludeFromCodeCoverage]
public static class ValidationExtensions
{
    /// <summary>Configures <see cref="ApiBehaviorOptions"/> so invalid model state returns a snake_cased, field-level error envelope.</summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection, for chaining.</returns>
    // Replaces the framework's default 400 (ProblemDetails) with our ApiResponse.Fail envelope.
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // Flatten every field's errors into a tidy list for the `details` bag.
                var errors = context.ModelState
                    .Where(kvp => kvp.Value is not null && kvp.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value!.Errors.Select(e => new
                    {
                        field = JsonNamingPolicy.SnakeCaseLower.ConvertName(kvp.Key),
                        error = string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value."
                            : e.ErrorMessage
                    }))
                    .ToList();

                var body = ApiResponse<object>.Fail(
                    "validation_error",
                    "One or more fields are invalid.",
                    errors);

                return new BadRequestObjectResult(body);   // 400 + your envelope
            };
        });

        return services;
    }
}
