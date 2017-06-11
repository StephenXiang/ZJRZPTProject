using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GeneralFrameworkDAL
{
    public class Net965808Service
    {
        public bool Login(string username, string pwd, out string userid)
        {
            var qurl = @"http://www.965808.gov.cn/encryptCredential.action";
            var timestamp = ((long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();
            var key = WebHelper.Get(qurl, string.Format("timestamp={0}", timestamp));
            var url = @"http://www.965808.gov.cn/mobile/userLogin.action";
            var pwde = Encrypter.EncryptMd5(pwd);
            var para = string.Format("userName={0},password={1}", username, pwde);
            var parae = Encrypter.EncryptDes2(para);
            Console.WriteLine("parae={0}", parae);
            var ret = WebHelper.Get(url, "para=" + parae);
            var reto = JsonConvert.DeserializeObject<Net965808Ret>(ret);
            userid = reto.userId;
            return reto.IsOk();
        }

        public bool Regist(string username, string pwd)
        {
            var url = @"http://www.965808.gov.cn/mobile/register.action";
            var paras = new Dictionary<string, string>
            {
                {"loginId", username},
                {"password", Encrypter.EncryptMd5(pwd)},
                {"displayName", username},
                {"duty", ""},
                {"mobile", ""}
            };
            var ret = WebHelper.Get(url, paras);
            var reto = JsonConvert.DeserializeObject<Net965808Ret>(ret);
            return reto.IsOk();
        }

        public bool Modify(string username, string newPassword, string oldPassword, string telephone = "", string displayName = "")
        {
            var url = @"http://www.965808.gov.cn/mobile/userInfoModify.action";
            var paras = new Dictionary<string, string>
            {
                {"userName", username},
                {"newPassword", Encrypter.EncryptMd5(newPassword)},
                {"oldPassword", Encrypter.EncryptMd5(oldPassword)},
                {"telephone", telephone},
                {"displayName", displayName}
            };
            var ret = WebHelper.Get(url, paras);
            var reto = JsonConvert.DeserializeObject<Net965808Ret>(ret);
            return reto.IsOk();
        }
    }

    public class Net965808Ret
    {
        public string message;

        public string code;

        public string userId;

        public bool IsOk()
        {
            return code == "0";
        }
    }
}
