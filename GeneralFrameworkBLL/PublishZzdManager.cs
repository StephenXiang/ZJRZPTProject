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

        public string GetBanks()
        {
            return _zs.GetBanks();
        }

        public string GetMastBanks()
        {
            return _zs.GetMastBanks();
        }

        public string GetZZDLBJson(string user, int page, int rows)
        {
            return _zs.GetZZDLBJson(user, page, rows);
        }

        public bool Delete(int id)
        {
            return _zs.Delete(id);
        }
    }
}
