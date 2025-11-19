using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.Shared.Contracts;

namespace AutomotiveStock.CentralStock.Data.Repositories
{
    public interface IStockRepository
    {
        void InitializeDatabaseSeed();
        void UpdateStockFromConsumption(string materialCode, double consumeQty);
        void UpdateStockFromReplenishment(MaterialReplenishmentEvent replenishmentEvent);
    }
}