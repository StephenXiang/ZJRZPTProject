using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace GeneralFrameworkDAL
{
    [TestFixture]
    class DalTester
    {
        [Test]
        public void TestScarlar()
        {
            var sql = @"insert into RZFinanceYear(EnterpriseId,Total,Liabilities,SaleIncome,Receivable,RetainedProfits,[Year],Jinlirun)
values (1002,3,4,5,6,7,'2017',3)
SELECT @@IDENTITY";
            var obj = DBHelper.GetScalar(sql) as int?;
            Console.WriteLine(obj);
        }

        [Test]
        public void TestGetEnterpriseFinanceInfo()
        {
            EnterpriseService es = new EnterpriseService();
            var str = es.GetEnterpriseFinanceInfoByUserName("testqy");
            Console.WriteLine(str);
        }
    }
}
