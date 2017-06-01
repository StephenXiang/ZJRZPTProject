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
    }
}
