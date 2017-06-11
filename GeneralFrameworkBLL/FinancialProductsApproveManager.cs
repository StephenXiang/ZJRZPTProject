using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLL
{
    public class FinancialProductsApproveManager
    {
        FinancialProductsApproveService fs = new FinancialProductsApproveService();
        public string GetJRCPTableJson(int page, int rows)
        {
            return fs.GetJRCPTableJson(page, rows);
        }
        public string GetJRCPDetialById(int Id)
        {
            return fs.GetJRCPDetialById(Id);
        }
        public bool EditJRCPStatus(string status, int Id)
        {
            return fs.EditJRCPStatus(status, Id);
        }
    }
}
