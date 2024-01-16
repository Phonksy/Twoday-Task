namespace ZooAnimalManagmentSystem.Models
{
    public class Enclosure
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Size { get; set; }
        public string? Location { get; set; }
        public List<string>? Objects { get; set; }
        public List<Animal>? Animals { get; set; }
    }
}
