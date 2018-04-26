using System.Collections.Generic;

namespace app.Models.ToolViewModels{
    public class CreateViewModel{

        
        public Tool Tool { get; set; }
        public List<Status> Statuses { get; set; }
    }
}
