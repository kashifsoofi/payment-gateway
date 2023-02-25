namespace Payments.Infrastructure.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Payments.Infrastructure.Database;
    using TestContainers.Container.Abstractions.Hosting;
    using TestContainers.Container.Abstractions.Networks;
    using TestContainers.Container.Database.Hosting;
    using TestContainers.Container.Database.MySql;
    using Xunit;

    public class DatabaseFixture : IAsyncLifetime
    {
        private const string ServiceDatabaseName = "Payments";

        private readonly bool useServiceDatabase;

        private readonly MySqlContainer databaseContainer;
        private MigrationsContainer migrationsContainer;

        public DatabaseFixture()
        {
            this.useServiceDatabase = Debugger.IsAttached;
            if (!this.useServiceDatabase)
            {
                Environment.SetEnvironmentVariable("REAPER_DISABLED", true.ToString());
                this.databaseContainer = new ContainerBuilder<MySqlContainer>()
                    .ConfigureDockerImageName("mysql:5.7")
                    .ConfigureDatabaseConfiguration("root", "integration123", "integrationdefaultdb")
                    .Build();
            }
        }

        public IConnectionStringProvider ConnectionStringProvider { get; private set; }

        public async Task InitializeAsync()
        {
            DatabaseOptions databaseOptions;

            if (!this.useServiceDatabase)
            {
                await this.databaseContainer.StartAsync();

                this.migrationsContainer = new ContainerBuilder<MigrationsContainer>()
                    .ConfigureContainer((context, container) =>
                    {
                        var connectionString =
                            $"Server=localhost;Port={this.databaseContainer.GetMappedPort(MySqlContainer.DefaultPort)};database={this.databaseContainer.DatabaseName};uid={this.databaseContainer.Username};password={this.databaseContainer.Password};SslMode=None;";
                        container.Command = new List<string>
                        {
                            "-cs", connectionString,
                        };
                    })
                    .ConfigureNetwork((hostContext, builderContext) =>
                    {
                        return new NetworkBuilder<UserDefinedNetwork>()
                            .ConfigureNetwork((context, network) => { network.NetworkName = "host"; })
                            .Build();
                    })
                    .Build();

                await this.migrationsContainer.StartAsync();
                var exitCode = await this.migrationsContainer.GetExitCodeAsync();
                if (exitCode > 0)
                {
                    throw new Exception($"Database migrations failed");
                }

                databaseOptions = new DatabaseOptions
                {
                    Server = this.databaseContainer.GetDockerHostIpAddress(),
                    Port = this.databaseContainer.GetMappedPort(MySqlContainer.DefaultPort),
                    Database = ServiceDatabaseName,
                    Username = this.databaseContainer.Username,
                    Password = this.databaseContainer.Password,
                };
            }
            else
            {
                databaseOptions = new DatabaseOptions
                {
                    Server = "localhost",
                    Port = 10286,
                    Database = ServiceDatabaseName,
                    Username = "root",
                    Password = "Password123",
                };
            }

            this.ConnectionStringProvider = new ConnectionStringProvider(databaseOptions);
        }

        public async Task DisposeAsync()
        {
            if (!this.useServiceDatabase)
            {
                if (this.migrationsContainer != null)
                {
                    await this.migrationsContainer.StopAsync();
                }

                if (this.databaseContainer != null)
                {
                    await this.databaseContainer.StopAsync();
                }
            }
        }
    }
}