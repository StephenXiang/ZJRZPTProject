using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class PublishZzdManager
    {
        PublishZzdService _zs = new PublishZzdService();
        public bool Save(ZzdInfo zi)
        {
            return _zs.Save(zi);
        }

        public string GetBanks(string mainbankid)
        {
            return _zs.GetBanks(mainbankid);
        }

        public string GetMastBanks(string mainbankid)
        {
            return _zs.GetMastBanks(mainbankid);
        }

        public string GetZZDLBJson(string user, int page, int rows)
        {
            return _zs.GetZZDLBJson(user, page, rows);
        }

        public bool Delete(int id)
        {
            return _zs.Delete(id);
        }
        public string GetMainBank()
        {
            return _zs.GetMainBank();
        }
    }
}
