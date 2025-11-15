
using AutomotiveStock.Shared.Services;
using Serilog;
using AutomotiveStock.PlantSimulator.Services;


Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/plant_simulator_.txt", 
        rollingInterval: RollingInterval.Day, 
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

RabbitMQServices rabbitMQServices = null;

try
{
    rabbitMQServices = new RabbitMQServices("localhost");

    PlantSimulationService plantSimulationService = new PlantSimulationService(rabbitMQServices);

    plantSimulationService.RunSimulation();
   
}catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação do Simulador falhou inesperadamente.");
}
finally
{
    Log.Information("--- Simulador da Planta Automotiva (Publisher) ENCERRADO ---");
    Log.CloseAndFlush();
}