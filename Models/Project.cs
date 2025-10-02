using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Project
{
    public string ProjectId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaProyek { get; set; } = null!;

    public string Klien { get; set; } = null!;

    public DateOnly TanggalMulai { get; set; }

    public DateOnly TanggalSelesai { get; set; }

    public string PeranTim { get; set; } = null!;

    public string PenggunaanTeknologi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
