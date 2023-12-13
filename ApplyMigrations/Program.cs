﻿using ApplyMigrations;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

using (var serviceProvider = CreateServices()) {
    using (var scope = serviceProvider.CreateScope()) {
        // Put the database update into a scope to ensure
        // that all resources will be disposed.
        UpdateDatabase(scope.ServiceProvider);
    }
}

static ServiceProvider CreateServices() {
    return new ServiceCollection()
        // Add common FluentMigrator services
        .AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
            // Add SQLite support to FluentMigrator
            .AddPostgres11_0()
            // Set the connection string
            .WithGlobalConnectionString("Data Source=test.db")
            // Define the assembly containing the migrations
            .ScanIn(typeof(AddLogTable).Assembly).For.Migrations())
        // Enable logging to console in the FluentMigrator way
        .AddLogging(lb => lb.AddFluentMigratorConsole())
        // Build the service provider
        .BuildServiceProvider(false);
}

/// <summary>
/// Update the database
/// </summary>
 static void UpdateDatabase(IServiceProvider serviceProvider) {
    // Instantiate the runner
    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

    // Execute the migrations
    runner.MigrateUp();
}