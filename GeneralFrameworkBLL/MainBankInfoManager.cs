using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLL
{
    public class MainBankInfoManager
    {
        MainBankInfoService _mb = new MainBankInfoService();
        public string GetMainBankCmb()
        {
            return _mb.GetMainBankCmb();
        }
    }
}
