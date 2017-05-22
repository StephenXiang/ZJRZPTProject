using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL;
using System.Data;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkBLL
{
    public class SysDepartmentManager
    {
        SysDepartmentService SDS = new SysDepartmentService();
        SysRolesManager SRM = new SysRolesManager();
        SysUserManager SUM = new SysUserManager();
        DataTable UserDt = new SysUserManager().GetUserDT();

        public string GetSysUserJson(string UserName)
        {
            string UserJson = string.Empty;
            DataTable UserInfoDt = SUM.GetUserInfoForUserName(UserName);
            DataTable DepartmentDT = SDS.GetSysDeparementDT();
            DataTable RolesDT = SRM.GetSysRolesDt();
            if (UserInfoDt.Rows.Count > 0)
            {
                DataRow dr = UserInfoDt.Rows[0];
                int RoleID = int.Parse(dr[1].ToString());
                if (RoleID != 1)
                {
                    DataRow[] dmarr = RolesDT.Select("ID <>'" + RoleID + "'");
                    for (int i = 0; i < dmarr.Length; i++)
                    {
                        RolesDT.Rows.Remove(dmarr[i]);
                    }
                }
                UserJson = JsonHelper.GetSysDepartmentJson(RolesDT, DepartmentDT);
                return UserJson;
            }
            else
            {
                return "";
            }
        }
    }
}
