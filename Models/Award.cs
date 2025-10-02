using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Award
{
    public string AwardId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaPenghargaan { get; set; } = null!;

    public string TingkatPenghargaan { get; set; } = null!;

    public string PemberiPenghargaan { get; set; } = null!;

    public int Tahun { get; set; }

    public string Deskripsi { get; set; } = null!;

    public string Sertifikat { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
