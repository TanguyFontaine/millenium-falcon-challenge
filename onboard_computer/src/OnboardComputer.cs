// Main file for the Onboard Computer service.

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var onboardComputerBuilder = WebApplication.CreateBuilder(args);

// Set file path from command line argument (first arg) or use default
if (args.Length > 0)
{
    OnboardComputerConfig.MillenniumFalconFilePath = args[0];
}

// Add CORS support for frontend
onboardComputerBuilder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

onboardComputerBuilder.Services.AddControllers();
var onboardComputerApp = onboardComputerBuilder.Build();

// Use CORS
onboardComputerApp.UseCors("AllowFrontend");

onboardComputerApp.UseRouting();
onboardComputerApp.MapControllers();

onboardComputerApp.Run("http://localhost:5001");

