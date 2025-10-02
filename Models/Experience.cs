using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Experience
{
    public string ExperienceId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Pengalaman { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
