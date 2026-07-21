namespace DocAnalytics.Service.Aws;

/// <summary>Strongly-typed AWS configuration (S3, Bedrock, region, and credentials).</summary>
public sealed class AwsOptions
{
    /// <summary>Primary AWS region (e.g. for S3).</summary>
    public string Region { get; set; } = "ap-south-1";
    /// <summary>Region used for Amazon Bedrock calls.</summary>
    public string BedrockRegion { get; set; } = "us-east-1";
    /// <summary>Target S3 bucket name.</summary>
    public string BucketName { get; set; } = null!;
    /// <summary>Bedrock Nova model id used for extraction.</summary>
    public string NovaModelId { get; set; } = "us.amazon.nova-lite-v1:0";
    /// <summary>AWS access key id (prefer task-role credentials in deployed environments).</summary>
    public string AccessKeyId { get; set; } = null!;
    /// <summary>AWS secret access key (prefer task-role credentials in deployed environments).</summary>
    public string SecretAccessKey { get; set; } = null!;
}
