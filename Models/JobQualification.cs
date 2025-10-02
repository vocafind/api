using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobQualification
{
    public string QualificationId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public string Kualifikasi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobVacancy Lowongan { get; set; } = null!;
}
