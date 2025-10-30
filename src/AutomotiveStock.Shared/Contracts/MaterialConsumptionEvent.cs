using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutomotiveStock.Shared.Enums;

namespace AutomotiveStock.Shared.Contracts
{
    public class MaterialConsumptionEvent
    {
        public Guid EventId { get; set; }
        public DateTime DateTimeConsumption { get; set; }
        public string PlantConsumption { get; set; }
        public int MaterialCode { get; set; }
        public double QtyConsumed { get; set; }
        public string ProductionOrder { get; set; }
        public ConsumedReasons ConsumedReason { get; set; }
    }
}