using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkDAL;
using System.Data;
using GeneralFrameworkDAL.JSON;

namespace GeneralFrameworkBLL
{
    public class SysUserManager
    {
        SysUserService Sus = new SysUserService();
        public bool Islogin(string username, string pwd)
        {
            bool islogin = false;
            if (Sus.getUserCount(username, pwd) == 1)
            {
                islogin = true;
            }
            return islogin;

        }

        public DataTable GetUserDT()
        {
            DataTable userdt = new DataTable();
            userdt = Sus.GetSysUserDT();
            return userdt;
        }

        public string GetUserTBJsonForDepartmentId(int DepartmentId)
        {
            DataTable dt = Sus.GetUserDTForDepartmentId(DepartmentId);
            string UserJson = JsonHelper.SerializeObject(dt);
            return UserJson;
        }

        public int EditUserPwd(string username, string pwd)
        {
            return Sus.EditUserPwd(username, pwd);
        }

        public DataTable GetUserInfoForUserName(string UserName)
        {
            return Sus.GetUserInfoForUserName(UserName);
        }
    }
}
