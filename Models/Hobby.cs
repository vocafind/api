using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace vocafind_api.Models;

public partial class Hobby
{
    public string HobbyId { get; set; } = null!;

    public string TalentId { get; set; } = null!;

    public string NamaHobi { get; set; } = null!;

    public string Deskripsi { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [JsonIgnore] // cegah loop
    public virtual Talent Talent { get; set; } = null!;
}
