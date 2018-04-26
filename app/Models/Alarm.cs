using System;

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
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int ToolId { get; set; }
        public Tool Tool { get; set; }
    }
}