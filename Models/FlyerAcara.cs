using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class FlyerAcara
{
    public ulong Id { get; set; }

    public ulong AcaraJobfairId { get; set; }

    public string FlyerUrl { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;
}
