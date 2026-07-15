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

public static class UploadsFeatureExtensions
{
    public static IServiceCollection AddInvoicePipeline(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<AwsOptions>(cfg.GetSection("Aws"));
        var aws = cfg.GetSection("Aws").Get<AwsOptions>()!;
        var creds = new BasicAWSCredentials(aws.AccessKeyId, aws.SecretAccessKey);

        services.AddSingleton<IAmazonS3>(_ =>
            new AmazonS3Client(creds, RegionEndpoint.GetBySystemName(aws.Region)));
        services.AddSingleton<IAmazonBedrockRuntime>(_ =>
            new AmazonBedrockRuntimeClient(creds, RegionEndpoint.GetBySystemName(aws.BedrockRegion)));

        services.AddScoped<IFileStorage, S3FileStorage>();
        services.AddScoped<IInvoiceExtractor, NovaInvoiceExtractor>();
        services.AddScoped<IInvoiceValidator, InvoiceValidator>();
        services.AddScoped<IUploadService, UploadService>();

        services.AddSingleton<IExtractionQueue, ExtractionQueue>();   // shared across requests + worker
        return services;
    }
}
