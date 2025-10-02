using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Training
{
    public string TrainingId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaPelatihan { get; set; } = null!;

    public string Penyelenggara { get; set; } = null!;

    public DateOnly TanggalMulai { get; set; }

    public DateOnly TanggalSelesai { get; set; }

    public string LinkSertifikat { get; set; } = null!;

    public string Deskripsi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
