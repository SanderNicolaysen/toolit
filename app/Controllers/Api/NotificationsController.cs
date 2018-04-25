using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using app.Data;
using app.Models;
using app.Services;

namespace app.Controllers_Api
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/Notifications")]
    public class NotificationsController : Controller
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly INotificationManager _nm;

        public NotificationsController(UserManager<ApplicationUser> um, INotificationManager nm)
        {
            _um = um;
            _nm = nm;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<IEnumerable<Notification>> GetNotifications()
        {
            return await _nm.GetNotificationsAsync(_um.GetUserId(User));
        }

        // GET: api/Notifications/Unread
        [HttpGet]
        [Route("Unread")]
        public async Task<IEnumerable<Notification>> GetUnreadNotifications()
        {
            return await _nm.GetUnreadNotificationsAsync(_um.GetUserId(User));
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<IActionResult> MarkAsRead([FromBody] Notification notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _nm.MarkAsReadAsync(notification);

            return Ok();
        }
    }
}