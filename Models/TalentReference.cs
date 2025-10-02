using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class TalentReference
{
    public string ReferenceId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string Nama { get; set; } = null!;

    public string Relasi { get; set; } = null!;

    public string? Perusahaan { get; set; }

    public string? Posisi { get; set; }

    public string Email { get; set; } = null!;

    public string Telepon { get; set; } = null!;

    public string? Deskripsi { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Talent Talent { get; set; } = null!;
}
