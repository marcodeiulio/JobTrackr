using FluentValidation;
using JobTrackr.API.Middleware;
using JobTrackr.Application.Common.Behaviors;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Common.Mappings;
using JobTrackr.Infrastructure;
using JobTrackr.Infrastructure.Data;
using JobTrackr.Infrastructure.Data.Seeding;
using JobTrackr.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
    .WriteTo.File("logs/jobtrackr-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllers();
    MappingConfig.RegisterMappings();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    // register MediatR
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IApplicationDbContext).Assembly));

    // register FluentValidation validators
    builder.Services.AddValidatorsFromAssembly(
        typeof(IApplicationDbContext).Assembly
    );

    // register ValidationBehavior
    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>)
    );

    // register LoggingBehavior
    builder.Services.AddTransient(
        typeof(IPipelineBehavior<,>),
        typeof(LoggingBehavior<,>)
    );

    builder.Services.AddOptions<JwtSettings>()
        .Bind(builder.Configuration.GetSection("Jwt"))
        .ValidateDataAnnotations()
        .ValidateOnStart();

    builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

    builder.Services.AddAuthorization();

    var app = builder.Build();

    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        using var scope = app.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        await RoleSeeder.SeedAsync(roleManager, logger);
        await JobApplicationStatusSeeder.SeedAsync(context, logger);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}