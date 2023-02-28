using Amazon.Runtime;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Autofac.Extensions.DependencyInjection;
using NServiceBus;
using Serilog;
using Serilog.Events;
using Payments.Api;
using Payments.Contracts.Messages.Commands;
using Payments.Infrastructure.Configuration;

var seqServerUrl = Environment.GetEnvironmentVariable("SEQ_SERVER_URL") ?? "http://localhost:5341";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl)
    .WriteTo.File("Payments.Api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

if (Environment.GetEnvironmentVariable("RUNNINGINCONTAINER") == "1")
{
    Log.Logger.Information("Running in container - delaying start");
    Thread.Sleep(TimeSpan.FromSeconds(30));
}

CreateHostBuilder(args)
    .UseNServiceBus(context =>
    {
        var configuration = context.Configuration;
        var nServiceBusOptions = configuration.GetSection("NServiceBus").Get<NServiceBusOptions>();

        var endpointConfiguration = new EndpointConfiguration("Payments.Api");

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(type => type.Namespace == "Payments.Contracts.Messages.Commands");
        conventions.DefiningEventsAs(type => type.Namespace == "Payments.Contracts.Messages.Events");
        conventions.DefiningMessagesAs(type => type.Namespace == "Payments.Infrastructure.Messages.Responses");

        var awsCredentials = new AnonymousAWSCredentials();
        var amazonSqsConfig = new AmazonSQSConfig();
        if (!string.IsNullOrEmpty(nServiceBusOptions.SqsServiceUrlOverride))
        {
            amazonSqsConfig.ServiceURL = nServiceBusOptions.SqsServiceUrlOverride;
        }
        var sqsClient = new AmazonSQSClient(awsCredentials, amazonSqsConfig);

        var amazonSnsConfig = new AmazonSimpleNotificationServiceConfig();
        if (!string.IsNullOrEmpty(nServiceBusOptions.SnsServiceUrlOverride))
        {
            amazonSnsConfig.ServiceURL = nServiceBusOptions.SnsServiceUrlOverride;
        }
        var snsClient = new AmazonSimpleNotificationServiceClient(awsCredentials, amazonSnsConfig);

        var amazonS3Config = new AmazonS3Config
        {
            ForcePathStyle = true,
        };
        if (!string.IsNullOrEmpty(nServiceBusOptions.S3ServiceUrlOverride))
        {
            amazonS3Config.ServiceURL = nServiceBusOptions.S3ServiceUrlOverride;
        }
        var s3Client = new AmazonS3Client(awsCredentials, amazonS3Config);

        var transport = new SqsTransport(sqsClient, snsClient)
        {
            // S3 = new S3Settings("bucket", "payments-api", s3Client),
        };
        var routing = endpointConfiguration.UseTransport(transport);
        routing.RouteToEndpoint(typeof(CreatePayment), "Payments-Host");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.EnableInstallers();

        endpointConfiguration.EnableCallbacks();
        endpointConfiguration.MakeInstanceUniquelyAddressable("1");

        endpointConfiguration.Recoverability()
            .Delayed(x => x.NumberOfRetries(0))
            .Immediate(x => x.NumberOfRetries(0));

        return endpointConfiguration;

    })
    .Build()
    .Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        })
        .UseServiceProviderFactory(new AutofacServiceProviderFactory());

public partial class Program
{ }