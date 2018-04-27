using System;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Log
    {
        public Log() {}

        public Log(int toolId, string userId, string fromDate, string toDate)
        {
            ToolId = toolId;
            UserId = userId;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public int Id { get; set; }

        [Required]
        public int ToolId { get; set; }

        public Tool Tool { get; set; }
        
        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Required]
        public string FromDate { get; set; }

        [Required]
        public string ToDate { get; set; }
    }
}