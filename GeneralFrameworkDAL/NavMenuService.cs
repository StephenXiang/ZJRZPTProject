using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using System.Data;

namespace GeneralFrameworkDAL
{
    public class NavMenuService
    {
        private DBHelper DBHelper = new DBHelper();
        SysUserService Sus = new SysUserService();
        public string GetNavMenuTableJson(string UserName)
        {
            DataTable dt = new DataTable();
            string sql = string.Empty;
            dt = Sus.GetUserInfoForUserName(UserName);
            DataRow dr = null;
            if (dt.Rows.Count > 0)
            {
                dr = dt.Rows[0];
            }
            sql = "select * from SysDepartmentMenu where DepartmentId = '" + dr[2].ToString() + "' order by menuID asc";
            dt = DBHelper.GetDataSet(sql);
            string MenuIds = string.Empty;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dr = dt.Rows[i];
                if (i == dt.Rows.Count - 1)
                {
                    MenuIds += dr[2].ToString();
                }
                else
                {
                    MenuIds += dr[2].ToString() + ",";
                }
            }
            string NavMenuJson = string.Empty;
            if (MenuIds != "")
            {
                sql = "select * from SysNavMenu where ID in(" + MenuIds + ")";
                dt = DBHelper.GetDataSet(sql);
                NavMenuJson = JSON.JsonHelper.SerializeObject(dt);
            }
            else
            {
                NavMenuJson = "";
            }

            return NavMenuJson;
        }

        public string GetSysMatMenuJson()
        {
            string sql = "select * from SysChildMenu where NavMenuId = 5";
            DataTable dt = DBHelper.GetDataSet(sql);
            string SysMatMenuJson = JSON.JsonHelper.SerializeObject(dt);
            return SysMatMenuJson;
        }

        public string GetALLSysMenuJson()
        {
            string sql = "select  ID,MenuName,MenuUrl,ParentMenuID from SysNavMenu order by ID asc";
            DataTable dt = DBHelper.GetDataSet(sql);
            string ALlSysMenuJson = JSON.JsonHelper.SerializeObject(dt);
            return ALlSysMenuJson;

        }
    }
}
