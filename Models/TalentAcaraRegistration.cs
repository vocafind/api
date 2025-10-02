using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class TalentAcaraRegistration
{
    public ulong Id { get; set; }

    public string TalentId { get; set; } = null!;

    public ulong AcaraJobfairId { get; set; }

    public string RegistrationCode { get; set; } = null!;

    public string? QrCodePath { get; set; }

    public string Status { get; set; } = null!;

    public string CheckinStatus { get; set; } = null!;

    public DateTime RegisteredAt { get; set; }

    public DateTime? AttendedAt { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public DateTime? CheckedOutAt { get; set; }

    public string? ScanLog { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual AcaraJobfair AcaraJobfair { get; set; } = null!;

    public virtual Talent Talent { get; set; } = null!;
}
