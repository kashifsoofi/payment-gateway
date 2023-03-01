﻿using Amazon;
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
using Prometheus;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Reflection;

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

// Start the metrics server on your preferred port number.
var port = configuration.GetValue<int>("Prometheus:Port");
var server = new KestrelMetricServer(port: port);
server.Start();

var host = CreateHostBuilder(args)
    .UseSerilog()
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddOptions();
        services.Configure<NServiceBusOptions>(configuration.GetSection("NServiceBus"));

        string applicationName = Assembly.GetExecutingAssembly().GetName().Name;
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName));
                builder.AddSource("MySqlConnector");
                builder.AddSource("NServiceBus.Core");
                builder.AddHttpClientInstrumentation();
                builder.AddConsoleExporter();
            });
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
        endpointConfiguration.EnableOpenTelemetry();

        var conventions = endpointConfiguration.Conventions();
        // conventions.DefiningCommandsAs(type => type.Namespace == "Payments.Contracts.Messages.Commands");
        conventions.DefiningEventsAs(type => type.Namespace == "Payments.Contracts.Messages.Events");
        conventions.DefiningMessagesAs(type => type.Namespace == "Payments.Infrastructure.Messages.Responses");

        var regionEndpoint = RegionEndpoint.GetBySystemName("eu-west-1");
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
        endpointConfiguration.UseTransport(transport);

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