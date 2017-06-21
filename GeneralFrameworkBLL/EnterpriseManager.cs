using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class EnterpriseManager
    {
        readonly EnterpriseService _es = new EnterpriseService();
        public bool Save(Enterprise ins, out string err)
        {
            return _es.Save(ins, out err);
        }

        public string LookUp(string key)
        {
            return _es.LookUp(key);
        }
        public int GetEntetpriseForName(string EnterpriseName, string UserName)
        {
            return _es.GetEntetpriseIdForName(EnterpriseName, UserName);
        }

        public string GetEnterpriseInfoForUserName(string UserName)
        {
            return _es.GetEnterpriseInfoForUserName(UserName);
        }

        public byte[] GetImgForCode(string code)
        {
            return _es.GetImgForCode(code);
        }

        public bool SaveEnterpriseFianceInfo(EnterpriseFinanceInfo efi)
        {
            return _es.SaveEnterpriseFianceInfo(efi);
        }

        public string GetEnterpriseFianceInfoByUserName(string username)
        {
            return _es.GetEnterpriseFinanceInfoByUserName(username);
        }
        public string GetEnterpriseInfoList()
        {
            return _es.GetEnterpriseInfoList();
        }
        public string GetEnterpriseInfoById(string id)
        {
            return _es.GetEnterpriseInfoById(id);
        }

        public string GetEnterpriseDg(int page,int rows)
        {
            return _es.GetEnterpriseDg(page, rows);
        }

        public string GetEnterpriseById(int id)
        {
            return _es.GetEnterpriseInfoForUserName(null, id);
        }

        public string GetEnterpriseFinanceInfoById(int id)
        {
            return _es.GetEnterpriseFinanceInfoByUserName(null, id);
        }
    }
}
