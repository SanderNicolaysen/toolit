using System.Collections.Generic;

namespace app.Models.AlarmViewModels{
    public class EditViewModel{
        public Alarm Alarm { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
