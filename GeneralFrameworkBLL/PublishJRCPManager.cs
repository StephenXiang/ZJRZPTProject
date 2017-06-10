﻿using System;
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
    }
}
