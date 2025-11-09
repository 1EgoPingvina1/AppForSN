namespace DTC.Domain.Entities.Main
{
    public class ProjectFile
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Backet { get; set; } = string.Empty; 
        public string OriginalName { get; set; } = string.Empty;
        public long Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public bool IsMainFile { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
