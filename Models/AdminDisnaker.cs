using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AdminDisnaker
{
    public string AdminDisnakerId { get; set; } = null!;

    public string AdminId { get; set; } = null!;

    public string Nik { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Admin Admin { get; set; } = null!;
}
