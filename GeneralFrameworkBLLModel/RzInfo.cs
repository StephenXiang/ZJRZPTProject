using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class RzInfo
    {
        public string UserName;
        public double RZED;
        public string RZYH;
        public int RZQX;
        public int RZQT;
        public string dywdesc;
    }
    public class RzInfoReply
    {
        public string UserName;
        public string RZED;
        public string RZYH;
        public string RZQX;
        public string RZQT;
        public string isdyw;
        public string dywdesc;
    }
}
