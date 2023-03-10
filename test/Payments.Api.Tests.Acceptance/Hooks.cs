using BoDi;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using PaymentGateway.ApiClient;
using Payments.Api.Tests.Acceptance.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Payments.Api.Tests.Acceptance
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer objectContainer;
        private readonly ScenarioContext scenarioContext;

        private static IConfigurationRoot configurationRoot;
        private static PaymentGatewayConfiguration paymentGatewayConfiguration = new PaymentGatewayConfiguration();

        public Hooks(IObjectContainer objectContainer, ScenarioContext scenarioContext)
        {
            this.objectContainer = objectContainer;
            this.scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appSettingsFilePath = Path.Combine(basePath, "appSettings.json");

            File.Exists(appSettingsFilePath).Should().BeTrue("appSettings.json file not found");

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder
                .AddJsonFile(appSettingsFilePath, false)
                .AddEnvironmentVariables();

            configurationRoot = configurationBuilder.Build();

            configurationRoot.GetSection(nameof(PaymentGatewayConfiguration)).Bind(paymentGatewayConfiguration);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            objectContainer.RegisterInstanceAs(new PaymentGatewayApiClient(new HttpClient
            {
                BaseAddress = new Uri(paymentGatewayConfiguration.BaseUrl),
            }));
        }

        [AfterScenario]
        public Task AfterScenario() => Task.CompletedTask;
    }
}
