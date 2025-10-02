using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobVacancy
{
    public string LowonganId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string Posisi { get; set; } = null!;

    public string DeskripsiPekerjaan { get; set; } = null!;

    public string? MinimalLulusan { get; set; }

    public string Status { get; set; } = null!;

    public string Lokasi { get; set; } = null!;

    public string Gaji { get; set; } = null!;

    public string JenisPekerjaan { get; set; } = null!;

    public DateOnly TanggalPosting { get; set; }

    public DateOnly BatasLamaran { get; set; }

    public int BatasPelamar { get; set; }

    public int JumlahPelamar { get; set; }

    public string TingkatPengalaman { get; set; } = null!;

    public bool OpsiKerjaRemote { get; set; }

    public string KontrakDurasi { get; set; } = null!;

    public string PeluangKarir { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<JobAdditionalFacility> JobAdditionalFacilities { get; set; } = new List<JobAdditionalFacility>();

    public virtual ICollection<JobAdditionalRequirement> JobAdditionalRequirements { get; set; } = new List<JobAdditionalRequirement>();

    public virtual ICollection<JobBenefit> JobBenefits { get; set; } = new List<JobBenefit>();

    public virtual ICollection<JobQualification> JobQualifications { get; set; } = new List<JobQualification>();

    public virtual ICollection<LowonganAcara> LowonganAcaras { get; set; } = new List<LowonganAcara>();
}
