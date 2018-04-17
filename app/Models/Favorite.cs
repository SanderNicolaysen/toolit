namespace app.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ToolId { get; set; }
        public Tool Tool { get; set; }
    }
}