namespace SHOTSPOT.Infrastructure.Settings;

public sealed class AwsSettings
{
    public const string SectionName = "AWS";

    public string Region { get; init; } = string.Empty;

    public string BucketName { get; init; } = string.Empty;

    public string AccessKey { get; init; } = string.Empty;

    public string SecretKey { get; init; } = string.Empty;

    public string? SessionToken { get; init; }
}