using AutoMapper;
using OrdersApi.DTOs;
using OrdersApi.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
