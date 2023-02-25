﻿namespace Payments.Host
{
    using Autofac;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Payments.Domain.Aggregates.Payment;
    using Payments.Infrastructure.AggregateRepositories.Payment;
    using Payments.Infrastructure.Database;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            var databaseOptions = Configuration.GetSection("Database").Get<DatabaseOptions>();
            builder.RegisterInstance(databaseOptions).As<IDatabaseOptions>().AsSelf().SingleInstance();
            builder.RegisterType<ConnectionStringProvider>().As<IConnectionStringProvider>().SingleInstance();

            builder.RegisterType<PaymentAggregateFactory>().As<IPaymentAggregateFactory>().SingleInstance();
            builder.RegisterType<PaymentRepository>().As<IPaymentAggregateReadRepository>().SingleInstance();
            builder.RegisterType<PaymentRepository>().As<IPaymentAggregateWriteRepository>().SingleInstance();
        }
    }
}
