using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class CooperativeBank
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public byte[] logo1 { get; set; }
        public byte[] logo2 { get; set; }
        public string BankDesc { get; set; }
        public int sort { get; set; }
        public string Leader { get; set; }
        public string Phone { get; set; }
        public int IsDesplay { get; set; }
    }
}
