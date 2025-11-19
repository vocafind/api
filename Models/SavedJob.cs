using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class SavedJob
{
    public string saved_job_ID { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string LowonganId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual JobVacancy Lowongan { get; set; } = null!;

    public virtual Talent Talent { get; set; } = null!;


}
