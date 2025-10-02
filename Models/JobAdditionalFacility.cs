using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobAdditionalFacility
{
    public string FacilityId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public string FasilitasTambahan { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobVacancy Lowongan { get; set; } = null!;
}
