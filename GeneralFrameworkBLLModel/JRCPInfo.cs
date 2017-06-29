using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class JRCPInfo
    {
        public string UserName;
        public string jrname;
        public int dkqxstart;
        public int dkqxend;
        public int jrdanbao;
        public double dkedstart;
        public double dkedend;
        public double? llfwstart;
        public double? llfwend;
        public string jrlxdh;
        public string jrcpjj;
        public string jrcptd;
        public string jrsykh;
        public string jrsqtj;
        public string jrcailiao;
        public byte[] logo;
    }

    public class JRCPReply
    {
        public string Title;
        public string lilvfanwei;
        public string daikuanedu;
        public string daikuanqixian;
        public string danbaofangshi;
        public string lianxidianhua;
        public string jianjie;
        public string tedian;
        public string shiyongkehu;
        public string tiaojian;
        public string cailiao;
    }
}
