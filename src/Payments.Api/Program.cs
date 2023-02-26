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

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Payments.Api.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

CreateHostBuilder(args)
    .UseNServiceBus(context =>
    {
        var configuration = context.Configuration;
        var nServiceBusOptions = configuration.GetSection("NServiceBus").Get<NServiceBusOptions>();

        var endpointConfiguration = new EndpointConfiguration("Payments.Api");
        endpointConfiguration.DoNotCreateQueues();

        var conventions = endpointConfiguration.Conventions();
        conventions.DefiningCommandsAs(type => type.Namespace == "Payments.Contracts.Messages.Commands");
        conventions.DefiningEventsAs(type => type.Namespace == "Payments.Contracts.Messages.Events");
        conventions.DefiningMessagesAs(type => type.Namespace == "Payments.Infrastructure.Messages.Responses");

        var amazonSqsConfig = new AmazonSQSConfig();
        if (!string.IsNullOrEmpty(nServiceBusOptions.SqsServiceUrlOverride))
        {
            amazonSqsConfig.ServiceURL = nServiceBusOptions.SqsServiceUrlOverride;
        }

        var transport = endpointConfiguration.UseTransport<SqsTransport>();
        transport.ClientFactory(() => new AmazonSQSClient(
            new AnonymousAWSCredentials(),
            amazonSqsConfig));

        var amazonSimpleNotificationServiceConfig = new AmazonSimpleNotificationServiceConfig();
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

        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(CreatePayment), "Payments-Host");

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