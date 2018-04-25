using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using app.Data;
using app.Models;

namespace app.Services
{
    public class NotificationManager : INotificationManager
    {
        private readonly ApplicationDbContext _db;

        public NotificationManager(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<Notification>> GetNotificationsAsync(string UserId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == UserId)
                .OrderBy(n => n.Time)
                .ToListAsync();
        }

        public async Task<List<Notification>> GetUnreadNotificationsAsync(string UserId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == UserId)
                .Where(n => n.IsRead == false)
                .OrderBy(n => n.Time)
                .ToListAsync();
        }

        public int UnreadCount(string userId)
        {
            return _db.Notifications
                .Where(n => n.UserId == userId)
                .Where(n => n.IsRead == false)
                .Count();
        }

        public async Task MarkAsReadAsync(Notification notification)
        {
            notification.IsRead = true;
            _db.Entry(notification).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task SendNotificationAsync(string userId, string message, string actionUrl)
        {
            var Notification = new Notification(userId, message, actionUrl);
            await _db.Notifications.AddAsync(Notification);
            await _db.SaveChangesAsync();
        }
    }
}