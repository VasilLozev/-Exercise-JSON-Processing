using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //9. Import Suppliers
            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //Console.WriteLine(ImportSuppliers(context, suppliersJson));

            //10. Import Parts
            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //Console.WriteLine(ImportParts(context, partsJson));

            //11. Import Cars
            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //Console.WriteLine(ImportCars(context, carsJson));

            //12. Import Customers
            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, customersJson));

            //13. Import Sales
            //string salesJson = File.ReadAllText("../../../Datasets/sales.json");
            //Console.WriteLine(ImportSales(context, salesJson));

            //14. Export Ordered Customers
            //Console.WriteLine(GetOrderedCustomers(context));

            //15. Export Cars from Make Toyota
            //Console.WriteLine(GetCarsFromMakeToyota(context));

            //16. Export Local Suppliers
            //Console.WriteLine(GetLocalSuppliers(context));

            //17. Export Cars with Their List of Parts
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            //18. Export Total Sales by Customer
            //Console.WriteLine(GetTotalSalesByCustomer(context));

            // 19. Export Sales with Applied Discount
            Console.WriteLine(GetSalesWithAppliedDiscount(context));
        }

        // 9. Import suppliers

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            SupplierDTO[] supplierDTOs = JsonConvert.DeserializeObject<SupplierDTO[]>(inputJson);

            Supplier[] suppliers = mapper.Map<Supplier[]>(supplierDTOs);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            PartsDTO[] partDTOs = JsonConvert.DeserializeObject<PartsDTO[]>(inputJson);
            List<Part> parts = new List<Part>();
            foreach (var partDTO in partDTOs)
            {
                if (context.Suppliers.Any(s => s.Id == partDTO.SupplierId))
                {
                    parts.Add(
                        mapper.Map<Part>(partDTO));
                }
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        //11. Import Cars

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            CarsDTO[] carDtos = JsonConvert.DeserializeObject<CarsDTO[]>(inputJson)!;

            List<Car> cars = new();

            foreach (var carDto in carDtos)
            {
                Car car = mapper.Map<Car>(carDto);

                foreach (int partId in carDto.partsId.Distinct())
                {
                    car.PartsCars.Add(new PartCar()
                    {
                        PartId = partId
                    });
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //11. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            CustomersDTO[] customersDTOs = JsonConvert.DeserializeObject<CustomersDTO[]>(inputJson);

            Customer[] customers = mapper.Map<Customer[]>(customersDTOs);

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Length}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<CarDealerProfile>());
            IMapper mapper = new Mapper(config);

            SalesDTO[] salesDTOs = JsonConvert.DeserializeObject<SalesDTO[]>(inputJson);

            Sale[] sales = mapper.Map<Sale[]>(salesDTOs);

            context.Sales.AddRange(sales);
            context.SaveChanges();
            return $"Successfully imported {sales.Length}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver != false);

            string output = JsonConvert.SerializeObject(
                customers.Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = c.IsYoungDriver,
                }),
                Formatting.Indented);
            return output;
        }
        //15. Export Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance);

            string output = JsonConvert.SerializeObject(cars.Select(c => new
            {
                c.Id,
                c.Make,
                c.Model,
                c.TraveledDistance
            }),
            formatting: Formatting.Indented);

            return output;
        }
        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    PartsCount = c.Parts.Count
                });

            string output = JsonConvert.SerializeObject(suppliers, Formatting.Indented);

            return output;
        }
        //17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var Cars = context.Cars
                 .Select(c => new
                 {
                     car = new
                     {
                         c.Make,
                         c.Model,
                         c.TraveledDistance
                     },
                     parts = c.PartsCars.Select(cp => new
                     {
                         Name = cp.Part.Name,
                         Price = $"{cp.Part.Price:f2}"
                     })
                 });
            string output = JsonConvert.SerializeObject (Cars, Formatting.Indented);
            return output;
        }
        //18. Export Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            string output = "";

            var customers = context.Customers
                .Where(c => c.Sales.Count() > 0).ToList();

            var finalCustomers = context.Customers
                .Select(c => new
                {
                    c.Id,
                    fullName = c.Name,
                    sales = c.Sales.Count(),
                    spentMoney = 0m,
                }).ToList();


            foreach (var customer in customers)
            {
                var spentMoney = 0m;
                foreach (var sale in customer.Sales)
                {
                    var discount = sale.Discount;
                    var carPrice = 0m;
                    foreach (var partsCars in sale.Car.PartsCars)
                    {
                        carPrice += partsCars.Part.Price;
                    }                    
                    var salePrice = carPrice - discount;

                    if (customer.IsYoungDriver)
                    {
                        salePrice -= 5/100 * salePrice;
                    }
                    spentMoney += salePrice;
                }
                var customer1 = context.Customers
                    .Select(c => new
                    {
                        c.Id,
                        fullName = c.Name,
                        sales = c.Sales.Count(),
                        spentMoney = spentMoney,
                    })
                    .FirstOrDefault(c => c.Id == customer.Id)
                    ;
                finalCustomers.Add(customer1);
            }
            var final1 = finalCustomers
                .Select(c => new
                {
                    c.fullName,
                    c.sales,
                    c.spentMoney
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenBy(c => c.sales);
            output += JsonConvert.SerializeObject(final1, Formatting.Indented);

            return output;
        }
        // 19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount,
                    price = s.Car.PartsCars.Sum(p => p.Part.Price),
                    priceWithDiscount = s.Car.PartsCars.Sum(p => p.Part.Price) - s.Discount

                });

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}