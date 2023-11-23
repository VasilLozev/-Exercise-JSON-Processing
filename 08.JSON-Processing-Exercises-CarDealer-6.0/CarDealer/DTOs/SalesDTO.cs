using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs
{
    public class SalesDTO
    {
        public decimal Discount { get; set; }

        public int CarId { get; set; }

        public int CustomerId { get; set; }
    }
}
