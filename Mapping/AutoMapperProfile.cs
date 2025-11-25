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








            /*---------------------------------------Jobfair-----------------------------------*/

            // Mapping untuk list jobfair
            CreateMap<AcaraJobfair, JobfairDTO>()
                .ForMember(dest => dest.FlyerUrl,
                    opt => opt.MapFrom(src => src.FlyerAcaras.FirstOrDefault().FlyerUrl))
                .ForMember(dest => dest.TotalLowongan,
                    opt => opt.MapFrom(src => src.LowonganAcaras.Count(la => la.Lowongan.Status == "aktif")))
                .ForMember(dest => dest.TotalPerusahaan,
                    opt => opt.MapFrom(src => src.AcaraJobfairCompanies.Count));


            // Mapping untuk detail jobfair
            CreateMap<AcaraJobfair, JobfairDetailDTO>()
                .ForMember(dest => dest.Perusahaan,
                    opt => opt.MapFrom(src => src.AcaraJobfairCompanies.Select(ajc => ajc.Company)))
                .ForMember(dest => dest.FlyerAcara,
                    opt => opt.MapFrom(src => src.FlyerAcaras))
                .ForMember(dest => dest.LowonganAcara,
                    opt => opt.MapFrom(src => src.LowonganAcaras.Where(la => la.Lowongan.Status == "aktif")));

            // Mapping untuk perusahaan di jobfair
            CreateMap<Company, CompanyJobfairDTO>()
                .ForMember(dest => dest.NamaPerusahaan,
                    opt => opt.MapFrom(src => src.NamaPerusahaan));

            // Mapping untuk flyer acara
            CreateMap<FlyerAcara, FlyerAcaraDTO>()
                .ForMember(dest => dest.FlyerUrl,
                    opt => opt.MapFrom(src => src.FlyerUrl));

            // Mapping untuk Lowongan di Jobfair
            CreateMap<JobVacancy, LowonganAcaraDTO>()
                .ForMember(dest => dest.LowonganId, opt => opt.MapFrom(src => src.LowonganId))
                .ForMember(dest => dest.Posisi, opt => opt.MapFrom(src => src.Posisi))
                .ForMember(dest => dest.DeskripsiPekerjaan, opt => opt.MapFrom(src => src.DeskripsiPekerjaan))
                .ForMember(dest => dest.MinimalLulusan, opt => opt.MapFrom(src => src.MinimalLulusan))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Lokasi, opt => opt.MapFrom(src => src.Lokasi))
                .ForMember(dest => dest.Gaji, opt => opt.MapFrom(src => src.Gaji))
                .ForMember(dest => dest.JenisPekerjaan, opt => opt.MapFrom(src => src.JenisPekerjaan))
                .ForMember(dest => dest.TanggalPosting, opt => opt.MapFrom(src => src.TanggalPosting))
                .ForMember(dest => dest.BatasLamaran, opt => opt.MapFrom(src => src.BatasLamaran))
                .ForMember(dest => dest.BatasPelamar, opt => opt.MapFrom(src => src.BatasPelamar))
                .ForMember(dest => dest.JumlahPelamar, opt => opt.MapFrom(src => src.JumlahPelamar))
                .ForMember(dest => dest.TingkatPengalaman, opt => opt.MapFrom(src => src.TingkatPengalaman))
                .ForMember(dest => dest.OpsiKerjaRemote, opt => opt.MapFrom(src => src.OpsiKerjaRemote))
                .ForMember(dest => dest.KontrakDurasi, opt => opt.MapFrom(src => src.KontrakDurasi))
                .ForMember(dest => dest.PeluangKarir, opt => opt.MapFrom(src => src.PeluangKarir))
                .ForMember(dest => dest.NamaPerusahaan, opt => opt.MapFrom(src => src.Company.NamaPerusahaan))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Company.Logo));

            // Mapping untuk Perusahaan di Jobfair (simplified)
            CreateMap<Company, CompanyJobfairDTO>()
                .ForMember(dest => dest.NamaPerusahaan, opt => opt.MapFrom(src => src.NamaPerusahaan))
                .ForMember(dest => dest.Logo, opt => opt.MapFrom(src => src.Logo));

            // Mapping utama untuk Jobfair Detail
            CreateMap<AcaraJobfair, JobfairDetailDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.NamaAcara, opt => opt.MapFrom(src => src.NamaAcara))
                .ForMember(dest => dest.AcaraBkk, opt => opt.MapFrom(src => src.AcaraBkk))
                .ForMember(dest => dest.AlamatAcara, opt => opt.MapFrom(src => src.AlamatAcara))
                .ForMember(dest => dest.Provinsi, opt => opt.MapFrom(src => src.Provinsi))
                .ForMember(dest => dest.Kabupaten, opt => opt.MapFrom(src => src.Kabupaten))
                .ForMember(dest => dest.Lokasi, opt => opt.MapFrom(src => src.Lokasi))
                .ForMember(dest => dest.TanggalAwalPendaftaranAcara, opt => opt.MapFrom(src => src.TanggalAwalPendaftaranAcara))
                .ForMember(dest => dest.TanggalAkhirPendaftaranAcara, opt => opt.MapFrom(src => src.TanggalAkhirPendaftaranAcara))
                .ForMember(dest => dest.TanggalMulaiAcara, opt => opt.MapFrom(src => src.TanggalMulaiAcara))
                .ForMember(dest => dest.TanggalSelesaiAcara, opt => opt.MapFrom(src => src.TanggalSelesaiAcara))
                .ForMember(dest => dest.Deskripsi, opt => opt.MapFrom(src => src.Deskripsi))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
                .ForMember(dest => dest.CurrentCapacity, opt => opt.MapFrom(src => src.CurrentCapacity))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Perusahaan, opt => opt.MapFrom(src => src.AcaraJobfairCompanies.Select(ajc => ajc.Company)))
                .ForMember(dest => dest.FlyerAcara, opt => opt.MapFrom(src => src.FlyerAcaras))
                .ForMember(dest => dest.LowonganAcara, opt => opt.MapFrom(src => src.LowonganAcaras.Select(la => la.Lowongan)));





            /*---------------------------------------Flyer Acara-----------------------------------*/

            // Mapping untuk request ke model
            CreateMap<FlyerAcaraRequestDTO, FlyerAcara>()
                .ForMember(dest => dest.FlyerUrl, opt => opt.Ignore()) // Diisi manual di controller
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

            // Mapping untuk model ke response
            CreateMap<FlyerAcara, FlyerAcaraResponseDTO>();

            // Mapping untuk FlyerAcaraDTO (jika diperlukan)
            CreateMap<FlyerAcara, FlyerAcaraDTO>()
                .ForMember(dest => dest.FlyerUrl, opt => opt.MapFrom(src => src.FlyerUrl));
        }
    }
}
