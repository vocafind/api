using AutoMapper;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Talent, TalentsRegisterDTO>().ReverseMap();
            CreateMap<Talent, TalentsUnverifiedDTO>().ReverseMap();
        }
    }
}
