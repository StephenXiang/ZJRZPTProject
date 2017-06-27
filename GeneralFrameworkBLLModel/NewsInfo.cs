using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class NewsInfo
    {
        public int NewsID;
        public string NewsTitle;
        public string NewsContent;
        public string NewsLink;
        public string CreateUser;

        public string NewsType;
        public int Relation_Firm;
        public DateTime Createdate;

        public byte[] image;
    }
}
