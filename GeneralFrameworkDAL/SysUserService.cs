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
        private Net965808Service _net965808Service = new Net965808Service();

        public SysUser Login(string username, string pwd, out string msg, bool onlylocal = false)
        {
            var sql = string.Format(@"select ID,UserPassWord,RolesID,OutPlatformId,IsEnable from SysUser where UserName='{0}'",
                username);
            var dt = DBHelper.GetDataSet(sql);
            SysUser li = null;
            if (dt.Rows.Count != 0)
            {
                li = new SysUser
                {
                    Id = Convert.ToInt32(dt.Rows[0]["ID"]),
                    RoleId = Convert.ToInt32(dt.Rows[0]["RolesID"]),
                    UserPassWord = dt.Rows[0]["UserPassWord"].ToString(),
                    OutId = dt.Rows[0]["OutPlatformId"].ToString(),
                    UserName = username,
                    IsEnable = Convert.ToInt32(dt.Rows[0]["IsEnable"])
                };
            }
            string outid;
            if (li != null)
            {
                if (li.IsEnable == 1)
                {
                    msg = "user is banned";
                    return null;
                }
                if (li.RoleId == 4)
                {
                    if (_net965808Service.Login(username, pwd, out outid, out msg))
                    {
                        if (pwd != li.UserPassWord || outid != li.OutId)
                        {
                            sql = string.Format(
                                "update SysUser set UserPassWord='{0}',OutPlatformId='{1}' where ID={2}",
                                pwd, outid, li.Id);
                            DBHelper.Execute(sql);
                        }
                        return li;
                    }
                    else if (msg == "密码错误")
                    {
                        return null;
                    }
                    else// 融资平台已有该企业账户，补注册965808
                    {
                        if (pwd != li.UserPassWord)
                        {
                            msg = "密码错误！";
                            return null;
                        }
                        if (Regist(username, pwd, out msg, true))
                        {
                            msg = "";
                            return new SysUser
                            {
                                DepartId = 5,
                                RoleId = 4
                            };
                        }
                        msg = "未注册965808！";
                        return null;
                    }
                }
                msg = "密码错误！";
                return pwd == li.UserPassWord ? li : null;
            }
            else
            {
                if (_net965808Service.Login(username, pwd, out outid, out msg))
                {
                    sql = @"
insert into SysUser (RolesID,DepartmentID,UserName,UserPassWord,IsEnable,OutPlatformId)
values(4,5,@un,@pwd,0,@oid)";
                    return DBHelper.Execute(sql,
                               new SqlParameter("@un", username),
                               new SqlParameter("@pwd", pwd),
                               new SqlParameter("@oid", outid)) > 0
                        ? new SysUser
                        {
                            DepartId = 5,
                            RoleId = 4
                        }
                        : null;
                }
                else
                {
                    return null;
                }
            }
            //@TODO TELEPHONE
        }

        // 注册企业用户接口
        public bool Regist(string username, string pwd, out string msg, bool ignoreExists = false)
        {
            if (!_net965808Service.Regist(username, pwd, out msg)) return false;
            string outid;
            if (!_net965808Service.Login(username, pwd, out outid, out msg))
            {
                msg = "965808平台注册失败！";
                return false;
            }
            var sql = string.Format("select ID from SysUser where UserName='{0}'", username);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count > 0)
            {
                if (ignoreExists)
                {
                    sql = string.Format(
                        "update SysUser set UserPassWord='{0}',OutPlatformId='{1}' where ID={2}",
                        pwd, outid, dt.Rows[0][0]);
                    DBHelper.Execute(sql);
                    return true;
                }
                msg = "用户名已存在";
                return false;
            }
            sql = @"
insert into SysUser (RolesID,DepartmentID,UserName,UserPassWord,IsEnable,OutPlatformId)
values(4,5,@un,@pwd,0,@oid)";
            return DBHelper.Execute(sql,
                       new SqlParameter("@un", username),
                       new SqlParameter("@pwd", pwd),
                       new SqlParameter("@oid", outid)) > 0;
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

        public DataTable GetUserDTForDepartmentId(int departmentId)
        {
            DataTable dt = new DataTable();
            string sql = "select a.ID,b.RolesName as UserRolesID,c.DepartmentName as UserDepartment,a.UserName as UserID,a.IsEnable as IsEnable from SysUser a  " +
                         "left join SysRoles b on a.RolesID = b.ID  " +
                        "left join SysDepartment c on a.DepartmentID = c.ID " +
                        "where c.ID = '" + departmentId + "'";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }

        public bool EditUserPwd(string userName, string pwd)
        {
            var sql = string.Format(@"select ID,RolesID,UserPassWord from SysUser where UserName='{0}'", userName);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count == 0) return false;
            var userid = Convert.ToInt32(dt.Rows[0][0]);
            var roleid = Convert.ToInt32(dt.Rows[0][1]);
            var oldpwd = dt.Rows[0][2].ToString();
            if (roleid == 4 && !_net965808Service.Modify(userName, pwd, oldpwd)) return false;
            sql = "update SysUser set UserPassWord = '" + pwd + "' where ID = " + userid;
            var i = DBHelper.GetNonQuery(sql);
            return i > 0;
        }

        public DataTable GetUserInfoForUserName(string userName)
        {
            DataTable dt = new DataTable();
            string sql = "select * from SysUser where UserName = '" + userName + "'";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }

        public bool DelUser(string userId)
        {
            string sql = @"update SysUser set IsEnable = 1 where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", userId));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool hfUser(string userId)
        {
            string sql = @"update SysUser set IsEnable = 0 where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", userId));
            if (sid > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Chongzhipwd(string userId, string pwd)
        {
            var sql = string.Format(@"select UserName,RolesID,UserPassWord from SysUser where ID={0}",
                userId);
            var dt = DBHelper.GetDataSet(sql);
            if (dt.Rows.Count == 0) return false;
            var userName = dt.Rows[0][0].ToString();
            var roleid = Convert.ToInt32(dt.Rows[0][1]);
            var oldpwd = dt.Rows[0][2].ToString();
            if (roleid == 4 && !_net965808Service.Modify(userName, pwd, oldpwd)) return false;
            sql = @"update SysUser set UserPassWord = '" + pwd + "' where ID =@userId";
            var sid = DBHelper.Execute(sql, new SqlParameter("@userId", userId));
            return sid > 0;
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

