using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

using app.Data;
using Microsoft.Extensions.DependencyInjection;

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

        public void CheckAlarms(Object state)
        {
            DateTime localTime = DateTime.Now;

            // If at least one of the alarm-dates have been surpassed, change the tool-status to "Not available" 

            using (IServiceScope scope = _ServiceProvider.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    
                var alarms = _db.Alarms.Include(a => a.Tool).ToList();
                foreach (var alarm in alarms)
                {
                    if (alarm.Date.CompareTo(localTime) < 0)
                        alarm.Tool.StatusId = _db.Statuses.Single(s => s.StatusName == "Busy").Id;
                }
                _db.SaveChangesAsync();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}