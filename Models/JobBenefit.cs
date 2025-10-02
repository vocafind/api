using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobBenefit
{
    public string BenefitId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public string Benefit { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobVacancy Lowongan { get; set; } = null!;
}
