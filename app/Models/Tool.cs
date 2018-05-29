using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace app.Models
{
    public class Tool
    {
        public Tool() { }

        public Tool(string name, Status status, List<Report> reports, List<Alarm> alarms, string alias, string shelf)
        {
            Name = name;
            StatusId = status.Id;
            Reports = reports;
            Alarms = alarms;
            Alias = alias;
            Shelf = shelf;
            CurrentOwnerId = "No owner";
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public string CurrentOwnerId { get; set; }
        public List<Report> Reports { get; set; }
        public List<Alarm> Alarms { get; set; }
        public string Alias { get; set; }
        public string Shelf { get; set; }

        [DisplayName("RFID code")]
        public string ToolIdentifierCode { get; set; }
    }
}