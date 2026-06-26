using DocAnalytics.Api.Common;
using Microsoft.AspNetCore.Mvc;

namespace DocAnalytics.Api.Extensions;

public static class ValidationExtensions
{
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
                        field = kvp.Key,
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
