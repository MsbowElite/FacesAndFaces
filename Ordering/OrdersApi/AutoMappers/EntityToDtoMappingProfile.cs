using AutoMapper;
using OrdersApi.DTOs;
using OrdersApi.Persistence.Entities;

namespace OrdersApi.AutoMappers
{
    public class EntityToDtoMappingProfile : Profile
    {
        public EntityToDtoMappingProfile()
        {
            CreateMap<OrderDetail, OrderDetailDTO>();
        }
    }
}
