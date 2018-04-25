using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using app.Data;
using app.Models;

namespace app.Services
{
    public interface INotificationManager
    {
        Task SendNotificationAsync(string userId, string message, string actionUrl);

        Task MarkAsReadAsync(Notification notification);

        int UnreadCount(string userId);

        Task<List<Notification>> GetNotificationsAsync(string UserId);

        Task<List<Notification>> GetUnreadNotificationsAsync(string UserId);
    }
}