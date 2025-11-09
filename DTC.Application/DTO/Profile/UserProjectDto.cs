namespace DTC.Application.DTO.Profile
{
    public class UserProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime VersionDate { get; set; }
        public int MemberCount { get; set; }
        public string Status { get; set; }
    }
}
