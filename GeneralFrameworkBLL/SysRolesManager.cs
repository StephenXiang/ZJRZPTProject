using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL;
using System.Data;

namespace GeneralFrameworkBLL
{
    public class SysRolesManager
    {
        SysRolesService SRS = new SysRolesService();
        public DataTable GetSysRolesDt()
        {
            return SRS.GetSysRolesDT();
        }
    }
}
