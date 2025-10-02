using System;
using System.Collections.Generic;

namespace vocafind_api.Models;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string HakAkses { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AdminCompany> AdminCompanies { get; set; } = new List<AdminCompany>();

    public virtual ICollection<AdminDisnaker> AdminDisnakers { get; set; } = new List<AdminDisnaker>();

    public virtual ICollection<AdminVokasi> AdminVokasis { get; set; } = new List<AdminVokasi>();
}
