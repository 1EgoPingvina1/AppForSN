namespace DTC.Domain.Entities.Main
{
    public class ProjectFile
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string StoragePath { get; set; } 
        public string OriginalName { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsMainFile { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
