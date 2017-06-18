#define NEWLOGIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL;
using System.Data;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkBLL
{
    public class SysUserManager
    {
        SysUserService Sus = new SysUserService();
        public SysUser Login(string username, string pwd, out string msg)
        {
#if NEWLOGIN
            return Sus.Login(username, pwd, out msg);
#else
            msg = "";
            return Sus.GetSysUserInfo(username, pwd);
#endif
        }

        public DataTable GetUserDT()
        {
            DataTable userdt = new DataTable();
            userdt = Sus.GetSysUserDT();
            return userdt;
        }

        public string GetUserTBJsonForDepartmentId(int DepartmentId, int page, int rows)
        {
            DataTable dt = Sus.GetUserDTForDepartmentId(DepartmentId);
            string UserJson = JsonHelper.TableToJson(dt.Rows.Count, JsonHelper.GetPagedTable(dt, page, rows));
            return UserJson;
        }

        public bool EditUserPwd(string username, string pwd)
        {
            return Sus.EditUserPwd(username, pwd);
        }

        public DataTable GetUserInfoForUserName(string UserName)
        {
            return Sus.GetUserInfoForUserName(UserName);
        }

        public bool DelUser(string UserId)
        {
            return Sus.DelUser(UserId);
        }
        public bool hfUser(string UserId)
        {
            return Sus.hfUser(UserId);
        }
        public bool chongzhipwd(string UserId, string pwd)
        {
            return Sus.Chongzhipwd(UserId, pwd);
        }
        public string GetSysRolesCmb()
        {
            return Sus.GetSysRolesCmb();
        }
        public string GetSysRoleId(string Did)
        {
            return Sus.GetSysRoleId(Did);
        }
        public string GetSysDeparementCmb(string RoleID)
        {
            return Sus.GetSysDeparementCmb(RoleID);
        }
        public bool AddUserInfo(SysUser user)
        {
            return Sus.AddUserInfo(user);
        }

        public bool Regist(string username, string md5pwd, out string msg)
        {
            return Sus.Regist(username, md5pwd, out msg);
        }
    }
}
