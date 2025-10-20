using AutoMapper;
using vocafind_api.DTO;
using vocafind_api.Models;

namespace vocafind_api.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            /*---------------------------------------Talent -----------------------------------*/
            CreateMap<Talent, TalentsRegisterDTO>().ReverseMap();
            CreateMap<Talent, TalentsUnverifiedDTO>().ReverseMap();




            /*---------------------------------------Loker Umum -----------------------------------*/
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





            /*---------------------------------------PERUSAHAAN-----------------------------------*/
            CreateMap<Company, PerusahaanDTO>();





            /*---------------------------------------SOCIAL MEDIA-----------------------------------*/
            CreateMap<Social, SocialGetDTO>();      // GET
            CreateMap<SocialPostDTO, Social>();     // POST
            CreateMap<SocialPutDTO, Social>();      // PUT


            /*---------------------------------------MINAT KARIR-----------------------------------*/
            CreateMap<CareerInterest, CareerInterestGetDTO>();      // GET
            CreateMap<CareerInterestPostDTO, CareerInterest>();     // POST
            CreateMap<CareerInterestPutDTO, CareerInterest>();      // PUT


            /*---------------------------------------TALENT REFERENCE-----------------------------------*/
            CreateMap<TalentReference, TalentReferenceGetDTO>();   // GET
            CreateMap<TalentReferencePostDTO, TalentReference>();  // POST
            CreateMap<TalentReferencePutDTO, TalentReference>();   // PUT




            /*---------------------------------------Education-----------------------------------*/
            CreateMap<Education, EducationGetDTO>();   // GET
            CreateMap<EducationPostDTO, Education>();  // POST
            CreateMap<EducationPutDTO, Education>();   // PUT
        }
    }
}
