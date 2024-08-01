namespace TFT_API.Models.Unit
{
    public class Skill
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public int StartingMana { get; set; }
        public int SkillMana { get; set; }
        public List<string> Stats { get; set; } = [];
    }
}
