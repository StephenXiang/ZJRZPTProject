using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class SysUser
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private SysRoles sysRols;

        internal SysRoles SysRols
        {
            get { return sysRols; }
            set { sysRols = value; }
        }

        private SysDepartment sysDepatment;

        internal SysDepartment SysDepatment
        {
            get { return sysDepatment; }
            set { sysDepatment = value; }
        }

        private string userName;

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private string userPassWord;

        public string UserPassWord
        {
            get { return userPassWord; }
            set { userPassWord = value; }
        }

        private int isEnable;

        public int IsEnable
        {
            get { return isEnable; }
            set { isEnable = value; }
        }
    }
}
