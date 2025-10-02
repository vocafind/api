using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class CareerPath
{
    public string CareerpathId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string JalurKarir { get; set; } = null!;

    public string Posisi { get; set; } = null!;

    public int Tingkatan { get; set; }

    public string Deskripsi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
