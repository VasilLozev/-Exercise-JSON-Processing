using AutoMapper;
using CarDealer.DTOs;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierDTO, Supplier>();
            CreateMap<PartsDTO, Part>();
            CreateMap<CarsDTO, Car>();
            CreateMap<CustomersDTO, Customer>();
            CreateMap<SalesDTO, Sale>();
        }
    }
}
