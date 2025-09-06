// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.Configure<CheckConfig>(builder.Configuration.GetSection(CheckConfig.Key));
builder.Services.Configure<GraphConfig>(builder.Configuration.GetSection(GraphConfig.Key));
builder.Services.AddSingleton<EmailService>();
builder.Services.AddLogging();
builder.Services.AddHostedService<CheckService>();
var host = builder.Build();

await host.RunAsync();

