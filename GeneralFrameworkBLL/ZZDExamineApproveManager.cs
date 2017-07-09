using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLL
{
    public class ZZDExamineApproveManager
    {
        ZZDExamineApproveService zs = new ZZDExamineApproveService();
        public string GetZZDDataTable(string UserName, int page, int rows)
        {
            return zs.GetZZDDataTable(UserName, page, rows);
        }

        public bool EditZZDStatus(string status, string zzdid)
        {
            return zs.EditZZDStatus(status, zzdid);
        }
        public string GetEnterpriseInfoForEnterpriseId(string enterpriseId)
        {
            return zs.GetEnterpriseInfoForEnterpriseId(enterpriseId);
        }
        public string GetEnterpriseFinanceInfoByEnterpriseId(string enterpriseId)
        {
            return zs.GetEnterpriseFinanceInfoByEnterpriseId(enterpriseId);
        }

        public string GetZZDInfoByZZDID(string zzdid)
        {
            return zs.GetZZDInfoByZZDID(zzdid);
        }
        public bool EditZZDStatus(string status, string zzdid, string liyou)
        {
            return zs.EditZZDStatus(status, zzdid, liyou);
        }
        public string GetBankInfoForUserName(string UserName)
        {
            return zs.GetBankInfoForUserName(UserName);
        }

        public string GetMastBankZZDDT(string UserName, int page, int rows)
        {
            return zs.GetMastBankZZDDT(UserName, page, rows);
        }
    }
}
