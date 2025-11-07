using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveStock.CentralStock.Data.Repositories
{
    public interface IStockRepository
    {
        void InitializeDatabaseSeed();
        void UpdateStockFromConsumption(string materialCode, double consumeQty);
    }
}