using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Language
{
    public string LanguageId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaBahasa { get; set; } = null!;

    public string Profisiensi { get; set; } = null!;

    public string? Sertifikat { get; set; }

    public string? Skor { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
