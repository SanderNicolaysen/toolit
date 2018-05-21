using System;
using System.ComponentModel;

namespace app.Models
{
    public class Alarm
    {
        public Alarm() {}

        public Alarm(string name, DateTime date)
        {
            Name = name;
            Date = date;
        }

        public int Id { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Date")]
        public DateTime Date { get; set; }
        public int ToolId { get; set; }
        [DisplayName("Tool")]
        public Tool Tool { get; set; }
        public int? StatusId { get; set; }
        public Status Status { get; set; }
    }
}