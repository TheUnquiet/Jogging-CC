using Azure.Storage.Blobs;
using Jogging.Api.Configuration;
using Jogging.Domain.Configuration;
using Jogging.Domain.DomainManagers;
using Jogging.Domain.Helpers;
using Jogging.Domain.Interfaces.RepositoryInterfaces;
using Jogging.Infrastructure2.Data;
using Jogging.Infrastructure2.Repositories.MySqlRepositories;
using Jogging.Rest.Controllers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Sinks.Discord;

namespace Jogging.Api;

internal class Program
{
    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var discordInformation = configuration.GetSection("Discord").Get<DiscordConfiguration>();

        if (discordInformation == null || discordInformation.WebhookId == 0 || string.IsNullOrWhiteSpace(discordInformation.WebhookToken))
        {
            throw new ApplicationException("Discord configuration is missing or invalid. Please check your WebhookId and WebhookToken.");
        }

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Discord(discordInformation.WebhookId, discordInformation.WebhookToken)
            .CreateLogger();

        try
        {
            Log.Information("Starting web host");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSingleton<CustomMemoryCache>();
            builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("StorageAccount")));
            builder.Services.AddScoped<IResultRepo, ResultRepo>();
            builder.Services.AddScoped<IClubRepo, ClubRepo>();
            builder.Services.AddScoped<ClubManager>();
            builder.Services.AddSingleton<BlobStorageController>();

            builder.Services.AddMemoryCache();

            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();
            var cs = builder.Configuration.GetConnectionString("MySql");
            builder.Services.AddDbContext<JoggingCcContext>(options =>
                options.UseMySql(cs, ServerVersion.AutoDetect(cs)));
            builder.Services.AddMultiSafepay(configuration);
            builder.Services.AddSmtpEmailClient(configuration);
            builder.Services.AddInterfaces();
            builder.Services.AddDomainManagerServices();
            builder.Services.AddHelperServices();

            builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            builder.Services.AddRateLimiter(RateLimiterConfigurator.ConfigureRateLimiter);

            builder.Services.AddXFrameSupress();

            builder.Services.AddCustomAuthentication(configuration);

            builder.Services.AddCors(CorsConfigurator.ConfigureCors);
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            HelmetConfig.AddHsts(app, builder);
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "jogging-api v1"); });
            }

            app.UseRateLimiter();

            app.UseHelmetHeaders();

            app.UseCors("AllowAny");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<JwtTokenValidationMiddleware>();
            app.UseMiddleware<RateLimitingMiddleware>("/api/auth/request-confirm-mail");

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}