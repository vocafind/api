using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobAdditionalRequirement
{
    public string RequirementId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public string PersyaratanTambahan { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobVacancy Lowongan { get; set; } = null!;
}
