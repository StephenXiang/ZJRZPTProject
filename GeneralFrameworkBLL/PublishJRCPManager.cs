using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using GeneralFrameworkDAL;

namespace GeneralFrameworkBLL
{
    public class PublishJRCPManager
    {
        private PublishJRCPService _js = new PublishJRCPService();
        public bool SaveJRCPInfo(JRCPInfo ji)
        {
            return _js.SaveJRCPInfo(ji);
        }

        public string GetJRCPLBJson(string username, int page, int rows)
        {
            return _js.GetJRCPJson(username, page, rows);
        }

        public string GetIndexJRCPInfo()
        {
            return _js.GetIndexJRCPInfo();
        }
        public string GetJRCPList(string dkqd = null, string dkqx = null, string dbfs = null, string dked = null, string jgmc = null, string cpmc = null)
        {
            return _js.GetJRCPList(dkqd, dkqx, dbfs, dked, jgmc, cpmc);
        }
        public string GetJRCPById(int id)
        {
            return _js.GetJRCPById(id);
        }

        public bool Delete(int id)
        {
            return _js.Delete(id);
        }
    }
}
