using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class CompanyBranch
{
    public string CompanybranchId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string NamaCabang { get; set; } = null!;

    public string Alamat { get; set; } = null!;

    public string NomorTelepon { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
