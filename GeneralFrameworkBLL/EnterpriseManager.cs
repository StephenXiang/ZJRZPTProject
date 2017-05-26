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
    }
}
