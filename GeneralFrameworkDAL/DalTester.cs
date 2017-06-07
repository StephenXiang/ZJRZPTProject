using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneralFrameworkBLLModel;
using Newtonsoft.Json;
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

        [Test]
        public void TestSaveJRCP()
        {
            PublishJRCPService ser = new PublishJRCPService();
            JRCPInfo ji = new JRCPInfo
            {
                UserName = "testyh",
                jrname = "testname",
                dkqxstart = 1,
                dkqxend = 2,
                jrdanbao = 10,
                dkedstart = 1.52,
                dkedend = 2.5,
                llfwstart = 1,
                llfwend = 1.01,
                jrlxdh = "18261952313",
                jrcpjj = "简介",
                jrcptd = "特点",
                jrsykh = "客户",
                jrsqtj = "条件",
                jrcailiao = "材料"
            };
            ser.SaveJRCPInfo(ji);
        }

        [Test]
        [Category("MANUAL")]
        public void TestSmsSend()
        {
            var smsRet = JsonConvert.DeserializeObject<SmsReturn>("{\"status\":100,\"count\":1,\"list\":[{\"p\":\"13615274683\",\"mid\":\"dfcaeca3820d5282\"}]}");
            Console.WriteLine("smsRet:{0}", smsRet.Status);
            var sms = new SmsService();
            sms.Send("");
        }
    }
}
