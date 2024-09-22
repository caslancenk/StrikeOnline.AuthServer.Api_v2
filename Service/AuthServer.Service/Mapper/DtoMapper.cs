using AuthServer.Core.Dtos;
using AuthServer.Core.Entity;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service.Mapper
{
    public class DtoMapper : Profile
    {
        public DtoMapper()
        {
            CreateMap<AppUserDto, AppUser>().ReverseMap();
            CreateMap<AppRoleDto, AppRole>().ReverseMap();
            CreateMap<ProductDto, Product>().ReverseMap();
        }
    }
}
