using ApplyMigrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

class Program {
    
    static void Main(string[] args) {

        string psqlConn = args[0];

        using (var serviceProvider = CreateServices(psqlConn)) {
            using (var scope = serviceProvider.CreateScope()) {
                // Put the database update into a scope to ensure
                // that all resources will be disposed.

                if (string.Compare(args[1].ToLower(), "list") == 0) {

                    ListDatabase(scope.ServiceProvider);
                }
                if (string.Compare(args[1].ToLower(), "up") == 0) {
                    UpdateDatabase(scope.ServiceProvider);
                }
            }
        }
    }

    private static ServiceProvider CreateServices(string psqlConn) {
        return new ServiceCollection()
            // Add common FluentMigrator services
            .AddFluentMigratorCore()
            .ConfigureRunner(rb =>
                // Add SQLite support to FluentMigrator
                rb.AddPostgres11_0()
                // Set the connection string
                .WithGlobalConnectionString(psqlConn)
                // Define the assembly containing the migrations
                .ScanIn(typeof(AddLogTable).Assembly)
                .For.Migrations()                
                .For.EmbeddedResources())
            // Enable logging to console in the FluentMigrator way
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            // Build the service provider
            .BuildServiceProvider(false);
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void UpdateDatabase(IServiceProvider serviceProvider) {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.MigrateUp();
    }

    /// <summary>
    /// Update the database
    /// </summary>
    private static void ListDatabase(IServiceProvider serviceProvider) {
        // Instantiate the runner
        var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

        // Execute the migrations
        runner.ListMigrations();
    }

}