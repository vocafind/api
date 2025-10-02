using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AcaraJobfairCompany
{
    public ulong Id { get; set; }

    public ulong AcaraJobfairId { get; set; }

    public string CompanyId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;
}
