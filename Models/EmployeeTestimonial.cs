using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class EmployeeTestimonial
{
    public string EmployeetestimonialsId { get; set; } = null!;

    public string CompanyId { get; set; } = null!;

    public string EmployeeName { get; set; } = null!;

    public string PositionTitle { get; set; } = null!;

    public int Rating { get; set; }

    public string Testimoni { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Company Company { get; set; } = null!;
}
