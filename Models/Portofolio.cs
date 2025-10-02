using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Portofolio
{
    public string PortfolioId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Judul { get; set; } = null!;

    public string Deskripsi { get; set; } = null!;

    public string? LinkPorotofolio { get; set; }

    public string? GaleriPortofolio { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
