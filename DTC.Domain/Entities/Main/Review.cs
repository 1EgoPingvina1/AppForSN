using DTC.Domain.Entities.Identity;

namespace DTC.Domain.Entities.Main
{
    public class Review
    {
        public int Id { get; set; }                         
        public int ProjectId { get; set; }                  
        public DateTime ReviewDate { get; set; }
        public int StateId { get; set; }                    
        public int ReasonId { get; set; }                   
        public string? Description { get; set; }
        public int RegUserId { get; set; }                  
        public DateTime RegDate { get; set; }

        public Project Project { get; set; } = null!;
        public State State { get; set; } = null!;
        public RefuseReason Reason { get; set; } = null!;
        public User RegUser { get; set; } = null!;
    }
}
