using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class TalentInterviewAttendance
{
    public ulong Id { get; set; }

    public string ApplyId { get; set; } = null!;

    public DateTime AttendedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobApply Apply { get; set; } = null!;
}
