using ElisBackend.Core.Application.UseCases;
using ElisBackend.Gateways.Dal;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Presenters.GraphQLSchema;
using GraphQL;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add AutoMapper to the container
builder.Services.AddAutoMapper(typeof(Program));

// Add services to the container.
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockHandling, StockHandling>();

// Add mediator and assemblies for mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add GraphQL
builder.Services.AddGraphQL(builder => builder
    .AddSystemTextJson()
    .AddSchema<ElisSchema>()
    .AddGraphTypes(typeof(ElisSchema).Assembly)
    );

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ElisDb med EntityFramework Core med PostgreSQL
builder.Services.AddDbContext<ElisContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("ElisDb")));
// TODO .UseSnakeCaseNamingConvention()

// Opret ExchangeService og registrer børs api'er 
// TODO

// Tillad alt for CORS
builder.Services.AddCors(options => {
    options.AddPolicy(
        name: "default",
        builder => {
            builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true)  // Vigtig
                .AllowCredentials();
        });
});

var app = builder.Build();

app.UseAuthorization();

app.UseCors("default");

app.UseDeveloperExceptionPage();

app.UseWebSockets();

app.UseGraphQL("/graphql");            // url to host GraphQL endpoint

app.UseGraphQLPlayground(
    "/",                               // url to host Playground at
    new GraphQL.Server.Ui.Playground.PlaygroundOptions {
        GraphQLEndPoint = "/graphql",         // url of GraphQL endpoint
        SubscriptionsEndPoint = "/graphql",   // url of GraphQL endpoint
    });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
