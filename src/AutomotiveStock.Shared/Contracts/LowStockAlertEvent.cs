using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveStock.Shared.Contracts
{
    public class LowStockAlertEvent
    {
        public Guid EventId { get; set; }
        public DateTime DateTimeLowStockAlert { get; set; }
        public string MaterialCode { get; set; }
        public double CurrentStock { get; set; }
        public double MinimumStock { get; set; }
        public double AverageDailyConsumption { get; set; }
        public int EstimatedNumberOfDays { get; set; }
    }
}