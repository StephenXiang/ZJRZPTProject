using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class PublishRzManager
    {
        readonly PublishRzService _prs = new PublishRzService();
        public string GetRzBanks()
        {
            return _prs.GetBanks();
        }

        public bool SaveRzInfo(RzInfo rzi)
        {
            return _prs.SaveRzInfo(rzi);
        }
        public string GetRZLBJson(string UserName)
        {
            return _prs.GetRZLBJson(UserName);
        }

        public string GetRZBankstr(string BankIds)
        {
            return _prs.GetRZBankstr(BankIds);
        }
    }
}
