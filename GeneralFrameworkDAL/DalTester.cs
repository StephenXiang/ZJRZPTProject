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
            sms.Send("18261952313", "test msg");
        }

        [Test]
        [Category("MANUAL")]
        public void TestUserLogin()
        {
            Net965808Service ns = new Net965808Service();
            string userid, msg;
            var user = "yinweiwen";
            var res = ns.Login(user, "Zxc123456", out userid, out msg, false) ? "登录成功" : "登录失败";
            Console.WriteLine("用户{0}{1},userId={2}", user, res, userid);
            user = "xiangpeng";
            res = ns.Login(user, "123456", out userid, out msg) ? "登录成功" : "登录失败";
            Console.WriteLine("用户{0}{1},userId={2}", user, res, userid);
        }

        [Test]
        [Category("MANUAL")]
        public void TestRegister()
        {
            Net965808Service ns = new Net965808Service();
            string msg;
            var res = ns.Regist("xiangpeng", "1234567", out msg, false) ? "注册成功" : "注册失败";
            Console.WriteLine(res);
        }

        [Test]
        [Category("MANUAL")]
        public void TestModify()
        {
            Net965808Service ns = new Net965808Service();
            var res = ns.Modify("xiangpeng", "123456", "1234567", "", "", false) ? "修改成功" : "修改失败";
            Console.WriteLine(res);
        }

        [Test]
        public void TestDeEncrypter()
        {
            var secret = @"MbT0uM2xs54lhPrwW/Z6DzOlyk0Dw39cUeHdxzi8qEz8Jf53RHivZ/g/8EXgObzEOlBwc00eQ2X7VUYwzNs4Rg==";
            var para = Encrypter.DecryptDes(secret);
            Console.WriteLine(para);
            secret = @"d3eb9a9233e52948740d7eb8c3062d14";
            para = Encrypter.EncryptMd5(secret);
            Console.WriteLine(para);
        }
    }
}
