using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneralFrameworkBLLModel
{
    public class Enterprise
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] BusinessLicense { get; set; }

        public string Code { get; set; }

        public int RegistTypeId { get; set; }

        public int ProfessionId { get; set; }

        public int EnterpriseTypeId { get; set; }

        public int RegistRegionId { get; set; }

        public int HuanpingId { get; set; }

        public double RegFinance { get; set; }

        public string RegFinanceMt { get; set; }

        public int Business { get; set; }

        public string MainProduction { get; set; }

        public DateTime CreateTime { get; set; }

        public string JuridicalPerson { get; set; }

        public string ConectionPerson { get; set; }

        public string ConnectionTelephone { get; set; }

        public string Desc { get; set; }

        public bool Outside { get; set; }
    }
}
