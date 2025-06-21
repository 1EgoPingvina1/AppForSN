namespace DTC.Domain.Entities.Main
{
    public class ProjectFile
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}
