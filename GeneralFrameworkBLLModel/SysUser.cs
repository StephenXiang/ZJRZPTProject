﻿namespace GeneralFrameworkBLLModel
{
    public class SysUser
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int DepartId { get; set; }

        public SysRoles SysRols { get; set; }

        public SysDepartment SysDepatment { get; set; }

        public string UserName { get; set; }

        public string UserPassWord { get; set; }

        public int IsEnable { get; set; }
    }
}
