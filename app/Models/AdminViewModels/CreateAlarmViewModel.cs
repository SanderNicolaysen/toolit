using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace app.Models.AdminViewModels
{
    public class CreateAlarmViewModel
    {
        [Required]
        [DisplayName("Alarmname")]
        public Alarm Alarm;
        
        public List<Tool> Tools { get; set; }
        public List<Status> Statuses { get; set; }
        
           
    }
}