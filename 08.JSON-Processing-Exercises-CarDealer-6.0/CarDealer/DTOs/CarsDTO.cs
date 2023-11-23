using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CarDealer.DTOs
{
    public class CarsDTO
    {
        public CarsDTO()
        {
            partsId = new HashSet<int>();
        }
        public string Make { get; set; } = null!;

        public string Model { get; set; } = null!;

        public long TraveledDistance { get; set; }
        public IEnumerable<int> partsId { get; set; }
    }
}
