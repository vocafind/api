using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AcaraJobfair
{
    public ulong Id { get; set; }

    public string NamaAcara { get; set; } = null!;

    public string? AcaraBkk { get; set; }

    public string? AlamatAcara { get; set; }

    public string? Provinsi { get; set; }

    public string? Kabupaten { get; set; }

    public string? Lokasi { get; set; }

    public string AdminVokasiId { get; set; } = null!;

    public DateOnly TanggalAwalPendaftaranAcara { get; set; }

    public DateOnly TanggalAkhirPendaftaranAcara { get; set; }

    public DateOnly TanggalMulaiAcara { get; set; }

    public DateOnly TanggalSelesaiAcara { get; set; }

    public string? Deskripsi { get; set; }

    public string Status { get; set; } = null!;

    public int MaxCapacity { get; set; }

    public int CurrentCapacity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AcaraInterviewSlot> AcaraInterviewSlots { get; set; } = new List<AcaraInterviewSlot>();

    public virtual ICollection<AcaraJobfairCompany> AcaraJobfairCompanies { get; set; } = new List<AcaraJobfairCompany>();

    public virtual ICollection<AdminSecurity> AdminSecurities { get; set; } = new List<AdminSecurity>();

    public virtual AdminVokasi AdminVokasi { get; set; } = null!;

    public virtual ICollection<ApplyAcara> ApplyAcaras { get; set; } = new List<ApplyAcara>();

    public virtual ICollection<FlyerAcara> FlyerAcaras { get; set; } = new List<FlyerAcara>();

    public virtual ICollection<LowonganAcara> LowonganAcaras { get; set; } = new List<LowonganAcara>();

    public virtual ICollection<LowonganPekerjaanAcara> LowonganPekerjaanAcaras { get; set; } = new List<LowonganPekerjaanAcara>();

    public virtual ICollection<TalentAcaraJobApplication> TalentAcaraJobApplications { get; set; } = new List<TalentAcaraJobApplication>();

    public virtual ICollection<TalentAcaraRegistration> TalentAcaraRegistrations { get; set; } = new List<TalentAcaraRegistration>();
}
