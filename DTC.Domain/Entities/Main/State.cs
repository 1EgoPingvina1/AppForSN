namespace DTC.Domain.Entities.Main
{
    public class State
    {
        public int Id { get; set; }                        
        public string Name { get; set; } = null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
