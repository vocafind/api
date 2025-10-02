using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class AdminSecurity
{
    public ulong AdminSecurityId { get; set; }

    public string Nim { get; set; } = null!;

    public string NamaLengkap { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string AdminVokasiId { get; set; } = null!;

    public ulong? AcaraJobfairId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair? AcaraJobfair { get; set; }

    public virtual AdminVokasi AdminVokasi { get; set; } = null!;
}
