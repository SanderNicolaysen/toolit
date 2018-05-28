using System.ComponentModel;

namespace app.Models
{
    public class Favorite
    {
        public string UserId { get; set; }
        
        [DisplayName("User")]
        public ApplicationUser User { get; set; }

        public int ToolId { get; set; }

        [DisplayName("Tool")]
        public Tool Tool { get; set; }
    }
}