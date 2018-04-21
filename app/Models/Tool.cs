using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Tool
    {
        public Tool() { }

        public Tool(string name, Status status, List<Report> reports, List<Alarm> alarms, string alias)
        {
            Name = name;
            StatusId = status.Id;
            Reports = reports;
            Alarms = alarms;
            Alias = alias;
        }

        public int Id { get; set; }
        public string Name { get; set; }
<<<<<<< HEAD
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
=======
        public string Status { get; set; }
        public bool CheckedOut { get; set; }
>>>>>>> Now if you press the check-out-button, the check-in-button becomes visible - and vice versa. It is stored in the database wether a tool is 'checked out' or not. Fixed some bugs.
        public List<Report> Reports { get; set; }
        public List<Alarm> Alarms { get; set; }
        public string Alias { get; set; }
    }
}