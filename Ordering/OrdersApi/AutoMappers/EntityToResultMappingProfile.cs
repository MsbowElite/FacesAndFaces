using AutoMapper;
using OrdersApi.Persistence.Entities;
using OrdersApi.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
