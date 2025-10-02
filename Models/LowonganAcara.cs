using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class LowonganAcara
{
    public ulong Id { get; set; }

    public string LowonganId { get; set; } = null!;

    public ulong AcaraJobfairId { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual JobVacancy Lowongan { get; set; } = null!;
}
