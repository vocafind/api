using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AcaraInterviewSlot
{
    public ulong Id { get; set; }

    public ulong AcaraJobfairId { get; set; }

    public string Slot { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual ICollection<JobApply> JobApplies { get; set; } = new List<JobApply>();
}
