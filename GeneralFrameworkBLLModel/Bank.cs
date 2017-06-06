using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class Bank
    {
        public int ID { get; set; }

        public string UserName { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Connector { get; set; }

        public string ConnectorPhone { get; set; }

        public string MainBankId { get; set; }
    }
}
