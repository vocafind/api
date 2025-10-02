using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Certification
{
    public string CertificationId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaSertifikasi { get; set; } = null!;

    public string LembagaSertifikasi { get; set; } = null!;

    public DateOnly TanggalTerbit { get; set; }

    public DateOnly TanggalHabisMasa { get; set; }

    public string? NomorSertifikat { get; set; }

    public string Sertifikat { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
