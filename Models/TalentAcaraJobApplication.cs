using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class TalentAcaraJobApplication
{
    public ulong Id { get; set; }

    public string TalentId { get; set; } = null!;

    public ulong LowonganAcaraId { get; set; }

    public ulong AcaraJobfairId { get; set; }

    public string ApplicationCode { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? InterviewSlot { get; set; }

    public string? CoverLetter { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual LowonganPekerjaanAcara LowonganAcara { get; set; } = null!;

    public virtual Talent Talent { get; set; } = null!;
}
