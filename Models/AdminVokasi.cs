using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AdminVokasi
{
    public string AdminVokasiId { get; set; } = null!;

    public string AdminId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Pt { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AcaraJobfair> AcaraJobfairs { get; set; } = new List<AcaraJobfair>();

    public virtual Admin Admin { get; set; } = null!;

    public virtual ICollection<AdminSecurity> AdminSecurities { get; set; } = new List<AdminSecurity>();

    public virtual ICollection<AlumniVokasi> AlumniVokasis { get; set; } = new List<AlumniVokasi>();
}
