using ElisBackend.Application.UseCases;
using ElisBackend.Gateways.Repositories.Stock;
using ElisBackend.Presenters.GraphQLSchema;
using GraphQL;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IStockRepository, StockRepository>();
builder.Services.AddSingleton<IStockHandling, StockHandling>();

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

// Tillad alle domæner for CORS
builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
