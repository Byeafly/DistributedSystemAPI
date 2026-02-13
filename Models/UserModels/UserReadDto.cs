namespace DistributedSystemAPI 
{ 
    public class UserReadDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string StreetAddress { get; set; } = null!;

        public Dictionary<string, string> Links { get; set; } = new();
    }
}
