﻿using System;
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
    }
}
