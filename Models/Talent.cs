using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Talent
{
    public string TalentId { get; set; } = null!;

    public string Nama { get; set; } = null!;

    public string? Nik { get; set; }

    public string StatusAkun { get; set; } = null!;

    public bool ResetPasswordRequest { get; set; }

    public int? KabupatenKotaId { get; set; }

    public string? KabupatenKota { get; set; }

    public int? ProvinsiId { get; set; }

    public string? Provinsi { get; set; }

    public int Usia { get; set; }

    public string JenisKelamin { get; set; } = null!;

    public string? Alamat { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RememberToken { get; set; }

    public string VerificationToken { get; set; } = null!;

    public string FotoProfil { get; set; } = null!;

    public string? TentangSaya { get; set; }

    public string NomorTelepon { get; set; } = null!;

    public int PreferensiGaji { get; set; }

    public string? LokasiKerjaDiinginkan { get; set; }

    public string? StatusPekerjaanSaatIni { get; set; }

    public TimeOnly? PreferensiJamKerjaMulai { get; set; }

    public TimeOnly? PreferensiJamKerjaSelesai { get; set; }

    public string StatusVerifikasi { get; set; } = null!;

    public string? PreferensiPerjalananDinas { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastVerificationRequestAt { get; set; }

    
    public virtual ICollection<Social> Socials { get; set; } = new List<Social>();
    public virtual ICollection<CareerInterest> CareerInterests { get; set; } = new List<CareerInterest>();
    public virtual ICollection<TalentReference> TalentReferences { get; set; } = new List<TalentReference>();


    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();
    public virtual ICollection<Language> Languages { get; set; } = new List<Language>();
    public virtual ICollection<Award> Awards { get; set; } = new List<Award>();


    public virtual ICollection<Certification> Certifications { get; set; } = new List<Certification>();
    public virtual ICollection<Training> Training { get; set; } = new List<Training>();
    public virtual ICollection<SoftSkill> SoftSkills { get; set; } = new List<SoftSkill>();
    public virtual ICollection<AdditionalSkill> AdditionalSkills { get; set; } = new List<AdditionalSkill>();


    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    public virtual ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<Hobby> Hobbies { get; set; } = new List<Hobby>();
    public virtual ICollection<Portofolio> Portofolios { get; set; } = new List<Portofolio>();



    public virtual ICollection<TalentAcaraJobApplication> TalentAcaraJobApplications { get; set; } = new List<TalentAcaraJobApplication>();

    public virtual ICollection<TalentAcaraRegistration> TalentAcaraRegistrations { get; set; } = new List<TalentAcaraRegistration>();

    public virtual ICollection<AlumniVokasi> AlumniVokasis { get; set; } = new List<AlumniVokasi>();

}
