using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace app.Models
{
    public class Notification
    {
        public Notification() {}

        public Notification(string userId, string message, string action)
        {
            this.UserId = userId;
            this.Message = message;
            this.ActionUrl = action;

            this.Time = System.DateTime.Now;
            this.IsRead = false;
        }

        public string Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [DisplayName("User")]
        public ApplicationUser User { get; set; }

        [Required]
        [DisplayName("Message")]
        public string Message { get; set; }

        [Required]
        [DisplayName("Time")]
        public DateTime Time { get; set; }

        [Required]
        public bool IsRead { get; set; }
        
        [Required]
        public string ActionUrl { get; set; }
    }
}