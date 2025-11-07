using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.CentralStock.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AutomotiveStock.CentralStock.Data.Repositories
{
    public class StockRepository : IStockRepository
    {

        private readonly DbContextOptions<CentralStockDbContext> _dbContext;

        public StockRepository(DbContextOptions<CentralStockDbContext> dbContextOptions)
        {
            _dbContext = dbContextOptions;
        }

        public void InitializeDatabaseSeed()
        {
            using (var dbContext = new CentralStockDbContext(_dbContext))
            {
                // Se a tabela não tiver dados, ele entra na condições -> ! indica o inverso do resultado
                if (!dbContext.Materials.Any())
                {
                    Log.Information("Banco de dados vazio. Iniciando o 'Seed' dos materiais...");

                    dbContext.Materials.Add(new Material
                    {
                        MaterialCode = "ACO-1020",
                        Name = "Aço ABNT 1020",
                        Type = "Steel",
                        Unit = "Kg",
                        MinimumStock = 10000,
                        Class = "A",
                        Supplier = "Gerdau",
                        CurrentStock = 20000
                    });

                    dbContext.Materials.Add(new Material
                    {
                        MaterialCode = "ALU-6061",
                        Name = "Alumínio 6061",
                        Type = "Aluminum",
                        Unit = "kg",
                        MinimumStock = 5000,
                        Class = "A",
                        Supplier = "Alcoa",
                        CurrentStock = 10000
                    });

                    dbContext.Materials.Add(new Material
                    {
                        MaterialCode = "PP-H301",
                        Name = "Polipropileno H301",
                        Type = "Plastic",
                        Unit = "kg",
                        MinimumStock = 3000,
                        Class = "B",
                        Supplier = "Braskem",
                        CurrentStock = 6000
                    });

                    dbContext.Materials.Add(new Material
                    {
                        MaterialCode = "BOR-NBR",
                        Name = "Borracha NBR",
                        Type = "Rubber",
                        Unit = "kg",
                        MinimumStock = 2000,
                        Class = "B",
                        Supplier = "Latex do Brasil",
                        CurrentStock = 4000
                    });

                    dbContext.Materials.Add(new Material
                    {
                        MaterialCode = "ACO-INOX",
                        Name = "Aço Inox 304",
                        Type = "Steel",
                        Unit = "kg",
                        MinimumStock = 4000,
                        Class = "A",
                        Supplier = "Aperam",
                        CurrentStock = 8000
                    });

                    dbContext.SaveChanges();
                    Log.Information("Seed de materiais concluído com sucesso.");
                }
                else
                {
                    Log.Information("Banco de dados já populado. 'Seed' não foi executado");
                }
            }
        }

        public void UpdateStockFromConsumption(string materialCode, double consumeQty)
        {
            using (var dbContext = new CentralStockDbContext(_dbContext))
            {
                var material = dbContext.Materials.FirstOrDefault(m => m.MaterialCode == materialCode);

                if (material != null)
                {
                    double oldStock = material.CurrentStock;
                    material.CurrentStock -= consumeQty;

                    dbContext.SaveChanges();

                    Log.Information("Estoque atualizado para {MaterialCode}: {OldStock} kg -> {NewStock} kg (-{ConsumeQty} kg)",
                    materialCode,
                    oldStock,
                    material.CurrentStock,
                    consumeQty
                    );
                }
                else
                {
                    Log.Warning("Recebido evento de consumo para material não cadastrado: {MaterialCode}", materialCode);
                }
            }
        }
    }
}