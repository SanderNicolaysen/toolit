using System.Collections.Generic;

namespace app.Models.AdminViewModels
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            ToolUseCount = new List<(Tool Tool, int Count)>(); 
            ToolUsage = new List<(Tool Tool, double UseagePercent)>(); 
            Log10 = new List<Log>();
        }

        public List<(Tool Tool, int Count)> ToolUseCount { get; set; }

        public List<(Tool Tool, double UsagePercent)> ToolUsage { get; set; }

        public List<Log> Log10 { get; set; }
    }
}