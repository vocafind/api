namespace vocafind_api.DTO
{
    // ✅ Untuk CREATE (POST)
    public class SoftSkillPostDTO
    {
        public string TalentId { get; set; } = null!;

        public string NamaSkill { get; set; } = null!;

        public string Profisiensi { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

    // ✅ Untuk GET (OUTPUT)
    public class SoftSkillGetDTO
    {
        public string SoftskillsId { get; set; } = null!;

        public string TalentId { get; set; } = null!;

        public string NamaSkill { get; set; } = null!;

        public string Profisiensi { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

    // ✅ Untuk PATCH (UPDATE)
    public class SoftSkillPutDTO
    {
        public string NamaSkill { get; set; } = null!;

        public string Profisiensi { get; set; } = null!;

        public string Deskripsi { get; set; } = null!;
    }

}
