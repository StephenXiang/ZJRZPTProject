using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace GeneralFrameworkDAL
{
    public class SysDepartmentService
    {
        public DataTable GetSysDeparementDT()
        {
            DataTable dt = new DataTable();
            string sql = "select ID,DepartmentName,DepartmentDec,RoleID from SysDepartment order by ID asc";
            dt = DBHelper.GetDataSet(sql);
            return dt;
        }
    }
}
