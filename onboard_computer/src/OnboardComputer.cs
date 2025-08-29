// Main file for the Onboard Computer service.

using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

var onboardComputerBuilder = WebApplication.CreateBuilder(args);
onboardComputerBuilder.Services.AddControllers();
var onboardComputerApp = onboardComputerBuilder.Build();

onboardComputerApp.UseRouting();
onboardComputerApp.MapControllers();

onboardComputerApp.Run("http://localhost:5001");
