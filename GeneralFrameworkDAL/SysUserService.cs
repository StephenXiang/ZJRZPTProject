using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GeneralFrameworkDAL
{
    public class SysUserService
    {
        public int getUserCount(string UserName, string Pwd)
        {
            int i = 0;
            string sql = "select ID from SysUser where UserName = '" + UserName + "' and UserPassWord = '" + Pwd + "' and isEnable = 0";
            DataTable dt = DBHelper.GetDataSet(sql);
            i = dt.Rows.Count;
            return i;
        }


        public DataTable GetSysUserDT()
        {
            DataTable dt = new DataTable();
            string sql = "select * from SysUser";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }

        public DataTable GetUserDTForDepartmentId(int DepartmentId)
        {
            DataTable dt = new DataTable();
            string sql = "select a.ID,b.RolesName as UserRolesID,c.DepartmentName as UserDepartment,a.UserName as UserID,a.IsEnable as IsEnable from SysUser a  " +
                         "left join SysRoles b on a.RolesID = b.ID  " +
                        "left join SysDepartment c on a.DepartmentID = c.ID " +
                        "where c.ID = '" + DepartmentId + "'";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }

        public int EditUserPwd(string UserName, string pwd)
        {
            string sql = "update SysUser set UserPassWord = '" + pwd + "' where UserName = '" + UserName + "'";
            int i = DBHelper.GetNonQuery(sql);
            return i;
        }

        public DataTable GetUserInfoForUserName(string UserName)
        {
            DataTable dt = new DataTable();
            string sql = "select * from SysUser where UserName = '" + UserName + "'";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }
    }
}
