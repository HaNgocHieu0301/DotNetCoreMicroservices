using AutoMapper;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.DTO;

namespace Mango.Services.OrderAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<OrderHeaderDTO, CartHeaderDTO>()
                .ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

                config.CreateMap<CartDetailsDTO, OrderDetailsDTO>()
                .ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price)).ReverseMap();

                config.CreateMap<OrderDetailsDTO, CartDetailsDTO>().ReverseMap();

                config.CreateMap<OrderHeader, OrderHeaderDTO>().ReverseMap();
                config.CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}