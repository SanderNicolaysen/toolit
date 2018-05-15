using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace app.Views.Admin
{
    public static class ManageNavPages
    {
        public static string ActivePageKey => "ActivePage";

        public static string Index => "Index";

        public static string Users => "Users";

        public static string Alarms => "Alarms";

        public static string Reports => "Reports";

        public static string Log => "Log";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string AlarmsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Alarms);
        
        public static string ReportsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Reports);

        public static string LogNavClass(ViewContext viewContext) => PageNavClass(viewContext, Log);
    
        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }

        public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
    }
}
