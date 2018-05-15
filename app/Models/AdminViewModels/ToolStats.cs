using System.Collections.Generic;

namespace app.Models.AdminViewModels
{
    public class ToolStats
    {
        public ToolStats()
        {
            ToolUseCount = new List<(Tool Tool, int Count)>(); 
            ToolUsage = new List<(Tool Tool, double UseagePercent)>(); 
        }

        public List<(Tool Tool, int Count)> ToolUseCount { get; set; }

        public List<(Tool Tool, double UsagePercent)> ToolUsage { get; set; }
    }
}