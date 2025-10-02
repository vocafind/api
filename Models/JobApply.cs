using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class JobApply
{
    public string ApplyId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public ulong? InterviewSlot { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? ApplicationCode { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual ICollection<ApplyAcara> ApplyAcaras { get; set; } = new List<ApplyAcara>();

    public virtual AcaraInterviewSlot? InterviewSlotNavigation { get; set; }

    public virtual ICollection<TalentInterviewAttendance> TalentInterviewAttendances { get; set; } = new List<TalentInterviewAttendance>();
}
