namespace DTC.Domain.Entities.Main
{
    public class ProjectType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Project> Projects { get; set; }

    }
}