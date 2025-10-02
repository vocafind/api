using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Social
{
    public string SocialId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Url { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
