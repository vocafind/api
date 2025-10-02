using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class ApplyAcara
{
    public ulong Id { get; set; }

    public string ApplyId { get; set; } = null!;

    public ulong AcaraJobfairId { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual JobApply Apply { get; set; } = null!;
}
