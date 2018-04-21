using System.Collections.Generic;

namespace app.Models
{
    public class Tool
    {
        public Tool() {}

        public Tool(string name, string status, List<Report> reports, List<Alarm> alarms)
        {
            Name = name;
            Status = status;
            Reports = reports;
            Alarms = alarms;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string ImagePath { get; set; }
        public List<Report> Reports { get; set; }
        public List<Alarm> Alarms { get; set; }
    }
}