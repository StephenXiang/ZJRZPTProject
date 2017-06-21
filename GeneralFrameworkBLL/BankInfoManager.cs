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
        public byte[] GetLogo2ImgForId(string Id)
        {
            return bi.GetLogo2ImgForId(Id);
        }

        public byte[] GetLogoImgForId(string Id)
        {
            return bi.GetLogoImgForId(Id);
        }

        public string GetMainBank(int page, int rows)
        {
            return bi.GetMainBank(page, rows);
        }

        public bool AddMainBank(string bankname)
        {
            return bi.AddMainBank(bankname);
        }
        public bool DelMainBank(int mainBankId)
        {
            return bi.DelMainBank(mainBankId);
        }
    }
}
