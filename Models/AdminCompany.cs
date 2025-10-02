using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AdminCompany
{
    public string AdminCompanyId { get; set; } = null!;

    public string AdminId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
