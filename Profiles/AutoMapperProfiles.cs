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
        }
    }
}
