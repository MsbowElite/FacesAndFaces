using AutoMapper;
using OrdersApi.Persistence.Entities;
using OrdersApi.Results;

namespace OrdersApi.AutoMappers
{
    public class EntityToResultMappingProfile : Profile
    {
        public EntityToResultMappingProfile()
        {
            CreateMap<Order, GetOrderByIdResult>();
        }
    }
}
