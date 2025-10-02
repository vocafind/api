using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace vocafind_api.Models;

public partial class Education
{
    public string EducationId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Institusi { get; set; } = null!;

    public string Jurusan { get; set; } = null!;

    public string Jenjang { get; set; } = null!;

    public string? Gelar { get; set; }

    public decimal NilaiAkhir { get; set; }

    public int TahunMasuk { get; set; }

    public int TahunLulus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore] // cegah loop
    public virtual Talent Talent { get; set; } = null!;
}
