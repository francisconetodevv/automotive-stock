using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.CentralStock.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutomotiveStock.CentralStock.Data
{
    public class CentralStockDbContext : DbContext
    {
        // A classe CentralStockDbContext é a responsável por 
        public CentralStockDbContext(DbContextOptions<CentralStockDbContext> options) : base(options)
        {
            
        }

        public DbSet<Material> Materials { get; set; }
    }
}