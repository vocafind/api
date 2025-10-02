using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class CareerInterest
{
    public string CareerinterestId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string TingkatKetertarikan { get; set; } = null!;

    public string Alasan { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string BidangKetertarikan { get; set; } = null!;

    public virtual Talent Talent { get; set; } = null!;
}
