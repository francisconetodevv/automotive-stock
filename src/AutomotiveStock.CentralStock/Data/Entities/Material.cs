using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutomotiveStock.CentralStock.Data.Entities
{
    public class Material
    {
        public int Id { get; set; }
        public string MaterialCode { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Unit { get; set; }
        public int MinimumStock { get; set; }
        public string Class { get; set; }
        public string Supplier { get; set; }
        public double CurrentStock { get; set; }

        public bool AlertSent { get; set; }
        
    }
}