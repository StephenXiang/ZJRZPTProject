using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class JRCPApply
    {
        public int Id { get; set; }
        public int JRCPID { get; set; }
        public int ApplyEnterpriseId { get; set; }
        public string ApplyEnterpriseName { get; set; }
        public string Phone { get; set; }
        public string CreateDate { get; set; }
    }
}
