using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class CompanyFacility
{
    public string CompanyfacilityId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string NamaFasilitas { get; set; } = null!;

    public string? GaleriFoto { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
