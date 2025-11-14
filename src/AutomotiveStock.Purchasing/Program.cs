using AutomotiveStock.Purchasing.Services;
using AutomotiveStock.Shared.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/purchasing.txt", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
        .CreateLogger();

RabbitMQServices rabbitMQServices = null;

try
{
    Log.Information("--- Sistema Central de Compras INICIANDO ---");

    rabbitMQServices = new RabbitMQServices("localhost");

    var alertEventConsumer = new AlertEventConsumer(rabbitMQServices);
    alertEventConsumer.StartListening();
    Console.WriteLine("Pressione [Enter] para sair.");
    Console.ReadLine();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Aplicação do Sistema de Compras Falhou inesperadamente.");
}
finally
{
    Log.Information("--- Sistema Central de Compras ENCERRANDO ---");
    rabbitMQServices?.DisposeConsumer();
    Log.CloseAndFlush();
}
