using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Serilog;
using Serilog.Events;
using Payments.Host;
using Payments.Infrastructure.Configuration;

var seqServerUrl = Environment.GetEnvironmentVariable("SEQ_SERVER_URL") ?? "http://localhost:5341";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl)
    .WriteTo.File("Payments.Host.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", true, true)
              .AddEnvironmentVariables()
              .Build();

if (Environment.GetEnvironmentVariable("RUNNINGINCONTAINER") == "1")
{
    Log.Logger.Information("Running in container - delaying start");
    Thread.Sleep(TimeSpan.FromSeconds(30));
}

var host = CreateHostBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddOptions();
        services.Configure<NServiceBusOptions>(configuration.GetSection("NServiceBus"));
    })
    .ConfigureContainer((HostBuilderContext hostBuilderContext, ContainerBuilder builder) =>
    {
        var startup = new Startup(hostBuilderContext.Configuration);
        startup.ConfigureContainer(builder);
    })
    .UseNServiceBus(context =>
    {
        var nServiceBusOptions = configuration.GetSection("NServiceBus").Get<NServiceBusOptions>();

        var endpointConfiguration = new EndpointConfiguration("Payments.Host");
        endpointConfiguration.DoNotCreateQueues();

        var conventions = endpointConfiguration.Conventions();
        // conventions.DefiningCommandsAs(type => type.Namespace == "Payments.Contracts.Messages.Commands");
        conventions.DefiningEventsAs(type => type.Namespace == "Payments.Contracts.Messages.Events");
        conventions.DefiningMessagesAs(type => type.Namespace == "Payments.Infrastructure.Messages.Responses");

        var regionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1");

        var amazonSqsConfig = new AmazonSQSConfig();
        amazonSqsConfig.RegionEndpoint = regionEndpoint;
        if (!string.IsNullOrEmpty(nServiceBusOptions.SqsServiceUrlOverride))
        {
            amazonSqsConfig.ServiceURL = nServiceBusOptions.SqsServiceUrlOverride;
        }

        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.ClientFactory(() => new AmazonSQSClient(
            new AnonymousAWSCredentials(),
            amazonSqsConfig));

        var amazonSimpleNotificationServiceConfig = new AmazonSimpleNotificationServiceConfig();
        amazonSimpleNotificationServiceConfig.RegionEndpoint = regionEndpoint;
        if (!string.IsNullOrEmpty(nServiceBusOptions.SnsServiceUrlOverride))
        {
            amazonSimpleNotificationServiceConfig.ServiceURL = nServiceBusOptions.SnsServiceUrlOverride;
        }

        transport.ClientFactory(() => new AmazonSimpleNotificationServiceClient(
            new AnonymousAWSCredentials(),
            amazonSimpleNotificationServiceConfig));

        var amazonS3Config = new AmazonS3Config
        {
            ForcePathStyle = true,
        };
        if (!string.IsNullOrEmpty(nServiceBusOptions.S3ServiceUrlOverride))
        {
            amazonS3Config.ServiceURL = nServiceBusOptions.S3ServiceUrlOverride;
        }

        var s3Configuration = transport.S3("payments", "api");
        s3Configuration.ClientFactory(() => new AmazonS3Client(
            new AnonymousAWSCredentials(),
            amazonS3Config));

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        endpointConfiguration.Recoverability()
            .Delayed(x => x.NumberOfRetries(0))
            .Immediate(x => x.NumberOfRetries(0));

        return endpointConfiguration;
    })
    .UseConsoleLifetime();

await host.RunConsoleAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());