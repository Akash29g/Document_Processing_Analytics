using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Runtime;
using Amazon.S3;
using DocAnalytics.Service.Aws;
using DocAnalytics.Service.Extraction;
using DocAnalytics.Service.Storage;
using DocAnalytics.Service.Uploads;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class UploadsFeatureExtensions
{
    public static IServiceCollection AddInvoicePipeline(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<AwsOptions>(cfg.GetSection("Aws"));
        var aws = cfg.GetSection("Aws").Get<AwsOptions>()!;
        var region = RegionEndpoint.GetBySystemName(aws.Region);

        // Use static keys ONLY if real ones are configured (local dev via user-secrets).
        // In prod (ECS) the keys are unset/placeholder → fall back to the default
        // credential chain, which picks up the ECS task role automatically.
        var useStaticKeys = !string.IsNullOrWhiteSpace(aws.AccessKeyId)
                            && aws.AccessKeyId != "SET_VIA_USER_SECRETS";

        services.AddSingleton<IAmazonS3>(_ =>
            useStaticKeys
                ? new AmazonS3Client(new BasicAWSCredentials(aws.AccessKeyId, aws.SecretAccessKey), region)
                : new AmazonS3Client(region));


        services.AddSingleton<IAmazonBedrockRuntime>(_ =>
        useStaticKeys
        ? new AmazonBedrockRuntimeClient(
            new BasicAWSCredentials(aws.AccessKeyId, aws.SecretAccessKey),
            RegionEndpoint.GetBySystemName(aws.BedrockRegion))
        : new AmazonBedrockRuntimeClient(
            RegionEndpoint.GetBySystemName(aws.BedrockRegion)));


        services.AddScoped<IFileStorage, S3FileStorage>();
        services.AddScoped<IInvoiceExtractor, NovaInvoiceExtractor>();
        services.AddScoped<IInvoiceValidator, InvoiceValidator>();
        services.AddScoped<IUploadService, UploadService>();

        services.AddSingleton<IExtractionQueue, ExtractionQueue>();   // shared across requests + worker
        return services;
    }
}
