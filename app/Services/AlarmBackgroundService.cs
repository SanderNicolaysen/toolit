using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using app.Data;
using app.Models;

namespace app.Services
{
    internal class AlarmBackgroundService : IHostedService, IDisposable
    {
        private IServiceProvider _ServiceProvider;
        private Timer _timer;

        public AlarmBackgroundService(IServiceProvider ServiceProvider)
        {
            _ServiceProvider = ServiceProvider;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckAlarms, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        // If at least one of the alarm-dates have been surpassed, change the tool-status to "Not available" 
        public async void CheckAlarms(Object state)
        {
            DateTime localTime = DateTime.Now;

            using (IServiceScope scope = _ServiceProvider.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var _nm = scope.ServiceProvider.GetRequiredService<INotificationManager>();
                var _um = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var admins = await _um.GetUsersInRoleAsync("Admin");
                    
                var alarms = _db.Alarms.Include(a => a.Tool).ToList();
                foreach (var alarm in alarms)
                {
                    if (alarm.Date.CompareTo(localTime) < 0)
                    {
                        alarm.Tool.StatusId = _db.Statuses.Single(s => s.StatusName == "Busy").Id;
                        foreach (var admin in admins)
                        {
                            await _nm.SendNotificationAsync(admin.Id, 
                                "Alarmen <b>" + alarm.Name + "</b> på verktøyet <b>" + alarm.Tool.Name + "</b> har utløpt",
                                "/Tool/Details/" + alarm.ToolId);
                        }
                        _db.Remove(alarm);
                    }
                }
                await _db.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}