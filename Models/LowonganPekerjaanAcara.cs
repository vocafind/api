using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class LowonganPekerjaanAcara
{
    public ulong Id { get; set; }

    public string CompanyId { get; set; } = null!;

    public ulong AcaraJobfairId { get; set; }

    public string Posisi { get; set; } = null!;

    public string? DeskripsiPekerjaan { get; set; }

    public string? MinimalLulusan { get; set; }

    public string Status { get; set; } = null!;

    public string Lokasi { get; set; } = null!;

    public string Gaji { get; set; } = null!;

    public string JenisPekerjaan { get; set; } = null!;

    public DateOnly TanggalPosting { get; set; }

    public DateOnly BatasLamaran { get; set; }

    public int JumlahPelamar { get; set; }

    public int BatasPelamar { get; set; }

    public string TingkatPengalaman { get; set; } = null!;

    public bool OpsiKerjaRemote { get; set; }

    public string KontrakDurasi { get; set; } = null!;

    public string PeluangKarir { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<TalentAcaraJobApplication> TalentAcaraJobApplications { get; set; } = new List<TalentAcaraJobApplication>();
}
