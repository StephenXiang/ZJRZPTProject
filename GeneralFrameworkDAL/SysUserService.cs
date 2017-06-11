using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using GeneralFrameworkBLLModel;
using System.Data.SqlClient;
using GeneralFrameworkDAL.JSON;

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

        public SysUser GetSysUserInfo(string userName, string pwd)
        {
            var sql = string.Format(
                @"select u.ID as userId,u.RolesID,u.DepartmentID,d.DepartmentName,d.DepartmentDec,r.RolesName,r.RolesDec
from SysUser u,SysDepartment d,SysRoles r where u.DepartmentID=d.ID and u.RolesID=r.ID
and u.UserName='{0}' and u.UserPassWord='{1}' and u.IsEnable=0", userName, pwd);
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt == null || dt.Rows.Count == 0) return null;
            return new SysUser
            {
                UserName = userName,
                Id = dt.Rows[0].Field<int>("userId"),
                RoleId = dt.Rows[0].Field<int>("RolesID"),
                DepartId = dt.Rows[0].Field<int>("DepartmentID"),
            };
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

        public bool DelUser(string UserId)
        {
            string sql = @"update SysUser set IsEnable = 1 where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", UserId));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hfUser(string UserId)
        {
            string sql = @"update SysUser set IsEnable = 0 where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", UserId));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool chongzhipwd(string UserId, string pwd)
        {
            string sql = @"update SysUser set UserPassWord = '" + pwd + "' where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", UserId));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetSysRolesCmb()
        {
            var sql = "select ID as ID,RolesDec as TypeName from SysRoles where ID != 1";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public string GetSysDeparementCmb(string RoleID)
        {
            var sql = "select ID,DepartmentName as TypeName from SysDepartment where RoleID = '" + RoleID + "'";
            var dt = DBHelper.GetDataSet(sql);
            return JsonHelper.ConvertJosnData(dt);
        }

        public string GetSysRoleId(string Did)
        {
            var sql = "select RoleID from SysDepartment where ID = '" + Did + "'";
            DataTable dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                return dr[0].ToString();
            }
            else
            {
                return "";
            }
        }

        public bool AddUserInfo(SysUser user)
        {
            string sql = string.Empty;
            if (user.RoleId == 2)
            {
                sql = @"insert into SysUser(RolesID,DepartmentID,UserName,UserPassWord,IsEnable,Phone)values(@roleid,@DepartmentID,@UserName,@UserPassWord,@IsEnable,@Phone)";
                var sid = DBHelper.Execute(sql, new SqlParameter("@roleid", user.RoleId),
                    new SqlParameter("@DepartmentID", user.DepartId),
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@UserPassWord", user.UserPassWord),
                    new SqlParameter("@IsEnable", user.IsEnable),
                    new SqlParameter("@Phone", user.Phone));
                if (sid > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sql = @"insert into SysUser(RolesID,DepartmentID,UserName,UserPassWord,IsEnable)values(@roleid,@DepartmentID,@UserName,@UserPassWord,@IsEnable)";
                var sid = DBHelper.Execute(sql, new SqlParameter("@roleid", user.RoleId),
                    new SqlParameter("@DepartmentID", user.DepartId),
                    new SqlParameter("@UserName", user.UserName),
                    new SqlParameter("@UserPassWord", user.UserPassWord),
                    new SqlParameter("@IsEnable", user.IsEnable));
                if (sid > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

        }

        public static string GetUserDepartment(string user)
        {
            var sql = string.Format(@"select (case RolesID 
when '1' then '管理员' 
when '2' then (select DepartmentName from SysDepartment where ID=u.DepartmentID)
when '3' then (select Name from Bank where ID=u.BankId)
when '4' then (select Name from Enterprise where ID=u.EnterpriseId)
end) as depart
from SysUser u where u.UserName='{0}'", user);
            var dt = DBHelper.GetDataTable(sql, null);
            if (dt.Rows.Count == 0) return "";
            return dt.Rows[0][0].ToString();
        }
    }
}

