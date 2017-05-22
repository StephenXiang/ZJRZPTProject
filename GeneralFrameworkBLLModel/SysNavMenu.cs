using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    /// <summary>
    /// 导航菜单实体
    /// </summary>
    public class SysNavMenu
    {
        /// <summary>
        /// 自动增上列
        /// </summary>
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// 菜单名称
        /// </summary>
        private string menuName;

        public string MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        /// <summary>
        /// 菜单父级ID
        /// </summary>
        private int parentMenuID;

        public int ParentMenuID
        {
            get { return parentMenuID; }
            set { parentMenuID = value; }
        }
    }
}
