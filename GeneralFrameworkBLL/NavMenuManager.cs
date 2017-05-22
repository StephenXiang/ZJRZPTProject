using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class NavMenuManager
    {
        NavMenuService NavMenuService = new NavMenuService();
        public string GetNavMenuJson(string UserName)
        {
            return NavMenuService.GetNavMenuTableJson(UserName);
        }

        public string GetSysMatMenuJson()
        {
            return NavMenuService.GetSysMatMenuJson();
        }
    }
}
