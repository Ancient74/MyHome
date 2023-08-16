using Microsoft.AspNetCore.HttpOverrides;
using MyHomeWebApi;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var webApplicationOptions = new WebApplicationOptions() { ContentRootPath = AppContext.BaseDirectory, Args = args, ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName };
var builder = WebApplication.CreateBuilder(webApplicationOptions);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
});

string logPath = Path.Combine(Environment.GetEnvironmentVariable("MyHomePath") ?? "", "logs/AppLog.txt");
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(logPath)
    .CreateLogger();

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(42069, listenOptions =>
    {
        string certPath = Path.Combine(Environment.GetEnvironmentVariable("MyHomePath") ?? "", "server.pfx");
        string password = Environment.GetEnvironmentVariable("MyHomeCertPassword") ?? "";
        listenOptions.UseHttps(certPath, password);
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMyHomeApiService, MyHomeApiService>();
builder.Services.AddScoped<IAudioControllerSelectorService, AudioControllerSelectorService>();

builder.Host.UseWindowsService();

builder.Logging.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseMiddleware<AuthenticationMiddleware>();
}

app.UseWhen(context => context.Request.Path.ToString().Contains("/Audio"), app =>
{
    app.UseMiddleware<AudioControllerSelectorMiddleware>();
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
    ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
