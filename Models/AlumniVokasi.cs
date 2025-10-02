using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AlumniVokasi
{
    public ulong Id { get; set; }

    public string TalentId { get; set; } = null!;

    public string AdminVokasiId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AdminVokasi AdminVokasi { get; set; } = null!;

    public virtual Talent Talent { get; set; } = null!;
}
