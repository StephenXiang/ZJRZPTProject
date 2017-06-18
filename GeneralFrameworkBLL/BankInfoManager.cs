using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLL
{
    public class BankInfoManager
    {
        BankInfoService bi = new BankInfoService();
        public string SaveBankInfo(Bank bank)
        {
            return bi.SaveBankInfo(bank);
        }
        public string GetBankId(string userName)
        {
            return bi.GetBankId(userName);
        }
        public string GetBankInfolbForBankId(string BankId)
        {
            return bi.GetBankInfolbForBankId(BankId);
        }

        public string GetBankInfoList()
        {
            return bi.GetBankInfoList();
        }
    }
}
