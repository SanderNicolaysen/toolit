using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Report
    {
        public Report() {}

        public Report(int toolId, string error, string userId)
        {
            ToolId = toolId;
            Error = error;
            UserId = userId;
        }

        public int Id { get; set; }

        [Required]
        public int ToolId { get; set; }
        
        public Tool Tool { get; set; }
        public string Error { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool isResolved { get; set; }
    }
}