namespace DocAnalytics.Service.Aws;

public sealed class AwsOptions
{
    public string Region { get; set; } = "ap-south-1";
    public string BedrockRegion { get; set; } = "us-east-1";
    public string BucketName { get; set; } = null!;
    public string NovaModelId { get; set; } = "us.amazon.nova-lite-v1:0";
    public string AccessKeyId { get; set; } = null!;
    public string SecretAccessKey { get; set; } = null!;
}
