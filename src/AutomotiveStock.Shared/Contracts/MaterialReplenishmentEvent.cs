using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveStock.Shared.Contracts
{
    public class MaterialReplenishmentEvent
    {
        public Guid EventId { get; set; }
        public DateTime DateTimeEntry { get; set; }
        public string DestinyPlant { get; set; }
        public string MaterialCode { get; set; }
        public double QtyReceived { get; set; }
        public string BatchNumberSupplier { get; set; }
        
        public string? Invoce { get; set; }

        public MaterialReplenishmentEvent()
        {
            EventId = Guid.NewGuid();
        }
    }
}