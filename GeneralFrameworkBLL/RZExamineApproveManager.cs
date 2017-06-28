using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLL
{
    public class RZExamineApproveManager
    {
        RZExamineApproveService rs = new RZExamineApproveService();
        public string GetRZDataTable(string UserName, int page, int rows)
        {
            return rs.GetRZDataTable(UserName, page, rows);
        }
        public string GetRZInfoByRZID(string RZID, string UserName)
        {
            return rs.GetRZInfoByRZID(RZID, UserName);
        }

        public bool EditRZStatus(string status, string rzid, string UserName, string slje = null, string slrq = null, int i = 1)
        {
            return rs.EditRZStatus(status, rzid, UserName, slje, slrq, i);
        }
        public bool EditRZStatus(string status, string rzid, string liyou)
        {
            return rs.EditRZStatus(status, rzid, liyou);
        }
        public string GetBankIdByUserName(string UserName)
        {
            return rs.GetBankIdByUserName(UserName);
        }
    }
}
