using AutoMapper;
using vocafind_api.DTO;
using vocafind_api.Models;
using static vocafind_api.DTO.TalentsDTO;

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
                .ForMember(dest => dest.NamaPerusahaan,
                           opt => opt.MapFrom(src => src.Company.NamaPerusahaan))
                .ForMember(dest => dest.Logo,
                           opt => opt.MapFrom(src => src.Company.Logo)); // <-- tambahkan ini




            // Mapping utama
            CreateMap<JobVacancy, LokerUmumDetailDTO>()
                // --- Mapping data perusahaan ---
                .ForMember(dest => dest.NamaPerusahaan,
                    opt => opt.MapFrom(src => src.Company.NamaPerusahaan))
                .ForMember(dest => dest.Nib,
                    opt => opt.MapFrom(src => src.Company.Nib))
                .ForMember(dest => dest.Npwp,
                    opt => opt.MapFrom(src => src.Company.Npwp))
                .ForMember(dest => dest.BidangUsaha,
                    opt => opt.MapFrom(src => src.Company.BidangUsaha))
                .ForMember(dest => dest.Alamat,
                    opt => opt.MapFrom(src => src.Company.Alamat))
                .ForMember(dest => dest.Provinsi,
                    opt => opt.MapFrom(src => src.Company.Provinsi))
                .ForMember(dest => dest.Kota,
                    opt => opt.MapFrom(src => src.Company.Kota))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.Company.Email))
                .ForMember(dest => dest.NomorTelepon,
                    opt => opt.MapFrom(src => src.Company.NomorTelepon))
                .ForMember(dest => dest.Website,
                    opt => opt.MapFrom(src => src.Company.Website))
                .ForMember(dest => dest.Logo,
                    opt => opt.MapFrom(src => src.Company.Logo))
                .ForMember(dest => dest.DeskripsiPerusahaan,
                    opt => opt.MapFrom(src => src.Company.DeskripsiPerusahaan))
                .ForMember(dest => dest.JumlahKaryawan,
                    opt => opt.MapFrom(src => src.Company.JumlahKaryawan))
                .ForMember(dest => dest.KebijakanKerja,
                    opt => opt.MapFrom(src => src.Company.KebijakanKerja))
                .ForMember(dest => dest.BudayaPerusahaan,
                    opt => opt.MapFrom(src => src.Company.BudayaPerusahaan))
                .ForMember(dest => dest.JumlahProyekBerjalan,
                    opt => opt.MapFrom(src => src.Company.JumlahProyekBerjalan))

                //kualifikasi
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







            //---------------------------------------TALENT PROFIL---------------------------------------TALENT PROFIL---------------------------------------TALENT PROFIL 

            // DATA DIRI-----------------------------------
            CreateMap<Talent, TalentsGetDataDiriDTO>();


            /*---------------------------------------SOSMED-----------------------------------*/
            CreateMap<Social, SocialGetDTO>();      // GET
            CreateMap<SocialPostDTO, Social>();     // POST
            CreateMap<SocialPutDTO, Social>();      // PUT


            /*---------------------------------------MINAT KARIR-----------------------------------*/
            CreateMap<CareerInterest, CareerInterestGetDTO>();      // GET
            CreateMap<CareerInterestPostDTO, CareerInterest>();     // POST
            CreateMap<CareerInterestPutDTO, CareerInterest>();      // PUT


            /*---------------------------------------REFERENSI-----------------------------------*/
            CreateMap<TalentReference, TalentReferenceGetDTO>();   // GET
            CreateMap<TalentReferencePostDTO, TalentReference>();  // POST
            CreateMap<TalentReferencePutDTO, TalentReference>();   // PUT





            //---------------------------------------AKADEMIK---------------------------------------AKADEMIK---------------------------------------AKADEMIK---------------------------------------AKADEMIK---------------------------------------AKADEMIK

            /*---------------------------------------Pendidikan-----------------------------------*/
            CreateMap<Education, EducationGetDTO>();   // GET
            CreateMap<EducationPostDTO, Education>();  // POST
            CreateMap<EducationPutDTO, Education>();   // PUT


            /*---------------------------------------Bahasa-----------------------------------*/
            CreateMap<Language, LanguageGetDTO>();   // GET
            CreateMap<LanguagePostDTO, Language>();  // POST
            CreateMap<LanguagePutDTO, Language>();   // PUT


            /*---------------------------------------Penghargaan-----------------------------------*/
            CreateMap<Award, AwardGetDTO>();   // GET
            CreateMap<AwardPostDTO, Award>();  // POST
            CreateMap<AwardPutDTO, Award>();   // PUT




            //---------------------------------------KOMPETENSI---------------------------------------KOMPETENSI---------------------------------------KOMPETENSI---------------------------------------KOMPETENSI---------------------------------------KOMPETENSI
            
            /*---------------------------------------Sertifikasi-----------------------------------*/
            CreateMap<Certification, CertificationGetDTO>();   // GET
            CreateMap<CertificationPostDTO, Certification>();  // POST
            CreateMap<CertificationPutDTO, Certification>();   // PUT


            /*---------------------------------------Pelatihan-----------------------------------*/
            CreateMap<Training, TrainingGetDTO>();   // GET
            CreateMap<TrainingPostDTO, Training>();  // POST
            CreateMap<TrainingPutDTO, Training>();   // PUT


            /*---------------------------------------Soft skill-----------------------------------*/
            CreateMap<SoftSkill, SoftSkillGetDTO>();   // GET
            CreateMap<SoftSkillPostDTO, SoftSkill>();  // POST
            CreateMap<SoftSkillPutDTO, SoftSkill>();   // PUT



            //---------------------------------------PENGALAMAN---------------------------------------PENGALAMAN---------------------------------------PENGALAMAN---------------------------------------PENGALAMAN---------------------------------------PENGALAMAN

            /*---------------------------------------Riwayat kerja-----------------------------------*/
            CreateMap<WorkHistory, WorkHistoryGetDTO>();   // GET
            CreateMap<WorkHistoryPostDTO, WorkHistory>();  // POST
            CreateMap<WorkHistoryPutDTO, WorkHistory>();   // PUT


            /*---------------------------------------Proyek-----------------------------------*/
            CreateMap<Project, ProjectGetDTO>();   // GET
            CreateMap<ProjectPostDTO, Project>();  // POST
            CreateMap<ProjectPutDTO, Project>();   // PUT


            /*---------------------------------------Portofolio-----------------------------------*/
            CreateMap<Portofolio, PortofolioGetDTO>();   // GET
            CreateMap<PortofolioPostDTO, Portofolio>();  // POST
            CreateMap<PortofolioPutDTO, Portofolio>();   // PUT






            /*---------------------------------------Talent-----------------------------------*/
            CreateMap<TalentsUpdateDTO, Talent>()
            .ForMember(dest => dest.FotoProfil, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        }
    }
}
