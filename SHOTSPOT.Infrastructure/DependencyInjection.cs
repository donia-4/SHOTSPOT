using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SHOTSPOT.Application.Common.Interfaces;
using SHOTSPOT.Infrastructure.Astronomy;
using SHOTSPOT.Infrastructure.Data.Sites;
using SHOTSPOT.Infrastructure.Geospatial;
using SHOTSPOT.Infrastructure.Settings;
using SHOTSPOT.Infrastructure.Storage;

namespace SHOTSPOT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddSiteCatalog()
            .AddGeospatial()
            .AddAstronomy()
            .AddStorage(configuration)
            .AddCaching();

        return services;
    }

    private static IServiceCollection AddSiteCatalog(
        this IServiceCollection services)
    {
        services.AddSingleton<ISiteCatalog, JsonSiteCatalog>();

        return services;
    }

    private static IServiceCollection AddGeospatial(
        this IServiceCollection services)
    {
        services.AddScoped<IDemReader, GdalDemReader>();

        return services;
    }

    private static IServiceCollection AddAstronomy(
    this IServiceCollection services)
    {
        services.AddHttpClient<HorizonsClient>(client =>
        {
            client.BaseAddress = new Uri(
                "https://ssd.jpl.nasa.gov/api/");

            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddScoped<IEphemerisProvider>(
            provider => provider.GetRequiredService<HorizonsClient>());

        return services;
    }

    private static IServiceCollection AddStorage(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var awsSection = configuration.GetSection(
            AwsSettings.SectionName);

        services
            .AddOptions<AwsSettings>()
            .Bind(awsSection)
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.Region),
                "AWS:Region is required.")
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.BucketName),
                "AWS:BucketName is required.")
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.AccessKey),
                "AWS:AccessKey is required.")
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.SecretKey),
                "AWS:SecretKey is required.")
            .ValidateOnStart();

        var awsSettings = awsSection.Get<AwsSettings>()
            ?? throw new InvalidOperationException(
                "AWS configuration section is missing.");

        Amazon.Runtime.AWSCredentials credentials =
            string.IsNullOrWhiteSpace(awsSettings.SessionToken)
                ? new Amazon.Runtime.BasicAWSCredentials(
                    awsSettings.AccessKey,
                    awsSettings.SecretKey)
                : new Amazon.Runtime.SessionAWSCredentials(
                    awsSettings.AccessKey,
                    awsSettings.SecretKey,
                    awsSettings.SessionToken);

        var awsOptions = new Amazon.Extensions.NETCore.Setup.AWSOptions
        {
            Credentials = credentials,
            Region = Amazon.RegionEndpoint.GetBySystemName(
                awsSettings.Region)
        };

        services.AddDefaultAWSOptions(awsOptions);

        services.AddAWSService<Amazon.S3.IAmazonS3>();

        services.AddScoped<IDemStorage, S3DemStorage>();

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        return services;
    }
}
