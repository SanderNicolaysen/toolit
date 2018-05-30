using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace app.Models.StatusesViewModels{
    public class DeleteViewModel{
        public Status Status { get; set; }
        public List<Status> Statuses { get; set; }
        public List<Tool> Tools { get; set; }
        public List<Alarm> Alarms { get; set; }
    }
}
