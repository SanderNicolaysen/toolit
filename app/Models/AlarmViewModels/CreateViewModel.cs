using System.Collections.Generic;

namespace app.Models.AlarmViewModels{
    public class CreateViewModel{

        public CreateViewModel(){
            Alarm = new Alarm();
        }
        public Alarm Alarm { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
