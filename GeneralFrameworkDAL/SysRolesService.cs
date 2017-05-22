using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GeneralFrameworkDAL
{
    public class SysRolesService
    {
        public DataTable GetSysRolesDT()
        {
            DataTable dt = new DataTable();
            string sql = "select ID,RolesName,RolesDec from SysRoles order by ID asc";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }
    }
}
