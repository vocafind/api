using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Company
{
    public string CompanyId { get; set; } = null!;

    public string NamaPerusahaan { get; set; } = null!;

    public string? Nib { get; set; }

    public string? Npwp { get; set; }

    public string BidangUsaha { get; set; } = null!;

    public string Alamat { get; set; } = null!;

    public string Provinsi { get; set; } = null!;

    public string Kota { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string NomorTelepon { get; set; } = null!;

    public string? Website { get; set; }

    public string? Logo { get; set; }

    public string? DeskripsiPerusahaan { get; set; }

    public int? JumlahKaryawan { get; set; }

    public string? KebijakanKerja { get; set; }

    public string? BudayaPerusahaan { get; set; }

    public int? JumlahProyekBerjalan { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AcaraJobfairCompany> AcaraJobfairCompanies { get; set; } = new List<AcaraJobfairCompany>();

    public virtual ICollection<AdminCompany> AdminCompanies { get; set; } = new List<AdminCompany>();

    public virtual ICollection<CareerPath> CareerPaths { get; set; } = new List<CareerPath>();

    public virtual ICollection<CompanyBranch> CompanyBranches { get; set; } = new List<CompanyBranch>();

    public virtual ICollection<CompanyFacility> CompanyFacilities { get; set; } = new List<CompanyFacility>();

    public virtual ICollection<EmployeeTestimonial> EmployeeTestimonials { get; set; } = new List<EmployeeTestimonial>();

    public virtual ICollection<JobVacancy> JobVacancies { get; set; } = new List<JobVacancy>();

    public virtual ICollection<LowonganPekerjaanAcara> LowonganPekerjaanAcaras { get; set; } = new List<LowonganPekerjaanAcara>();
}
