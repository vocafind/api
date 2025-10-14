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

            CreateMap<JobVacancy, LokerUmumDTO>()
           .ForMember(dest => dest.CompanyName,
                      opt => opt.MapFrom(src => src.Company.NamaPerusahaan));

            /*CreateMap<JobVacancy, LokerUmumDetailDTO>()
            .ForMember(dest => dest.CompanyName,
                     opt => opt.MapFrom(src => src.Company.NamaPerusahaan));*/





            // Mapping utama
            CreateMap<JobVacancy, LokerUmumDetailDTO>()
                .ForMember(dest => dest.CompanyName,
                           opt => opt.MapFrom(src => src.Company.NamaPerusahaan))
                .ForMember(dest => dest.JobQualifications,
                           opt => opt.MapFrom(src => src.JobQualifications))
                .ForMember(dest => dest.JobBenefits,
                           opt => opt.MapFrom(src => src.JobBenefits))
                .ForMember(dest => dest.JobAdditionalRequirements,
                           opt => opt.MapFrom(src => src.JobAdditionalRequirements))
                .ForMember(dest => dest.JobAdditionalFacilities,
                           opt => opt.MapFrom(src => src.JobAdditionalFacilities));

            // Mapping tiap relasi ke DTO
            CreateMap<JobQualification, JobQualificationDTO>()
                .ForMember(dest => dest.Kualifikasi,
                           opt => opt.MapFrom(src => src.Kualifikasi));

            CreateMap<JobBenefit, JobBenefitDTO>()
                .ForMember(dest => dest.Benefit,
                           opt => opt.MapFrom(src => src.Benefit));

            CreateMap<JobAdditionalRequirement, JobAdditionalRequirementDTO>()
                .ForMember(dest => dest.Persyaratan,
                           opt => opt.MapFrom(src => src.PersyaratanTambahan));

            CreateMap<JobAdditionalFacility, JobAdditionalFacilityDTO>()
                .ForMember(dest => dest.Fasilitas,
                           opt => opt.MapFrom(src => src.FasilitasTambahan));

        }
    }
}
