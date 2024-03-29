using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Reservation
    {
        public Reservation() {}

        public Reservation(int toolId, string userId, DateTime fromDate, DateTime toDate)
        {
            ToolId = toolId;
            UserId = userId;
            FromDate = fromDate;
            ToDate = toDate;
        }

        public int Id { get; set; }

        [Required]
        public int ToolId { get; set; }
        
        [DisplayName("Tool")]
        public Tool Tool { get; set; }

        public string UserId { get; set; }

        [DisplayName("User")]
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }
    }
}