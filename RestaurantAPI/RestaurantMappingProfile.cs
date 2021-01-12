using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember(m => m.City, c => c.MapFrom(s => s.Address.City))
                .ForMember(m => m.Street, c => c.MapFrom(s => s.Address.Street))
                .ForMember(m => m.PostalCode, c => c.MapFrom(s => s.Address.PostalCode));

            CreateMap<Dish, DishDto>();
        }
    }
}
