using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class WorkHistory
{
    public string WorkhistoryId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Posisi { get; set; } = null!;

    public string Perusahaan { get; set; } = null!;

    public string TanggalMulai { get; set; } = null!;

    public string TanggalSelesai { get; set; } = null!;

    public string Deskripsi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
