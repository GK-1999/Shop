using AutoMapper;
using Shop.Models.DataModels;
using Shop.Models.ViewModels;

namespace Shop.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UserDetails, RegistrationModel>();
            CreateMap<Product , UpdateProduct>();
            CreateMap<Product, ViewProducts>();
        }
    }
}
