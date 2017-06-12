using System.Collections.Generic;
using Newtonsoft.Json;

namespace GeneralFrameworkDAL
{
    public class Net965808Service
    {
        private const string LoginUrl = @"http://www.965808.gov.cn/mobile/userLogin.action";

        private const string RegisterUrl = @"http://www.965808.gov.cn/mobile/register.action";

        private const string ModifyUrl = @"http://www.965808.gov.cn/mobile/userInfoModify.action";

        public bool Login(string username, string pwd, out string userid)
        {
            var pwde = Encrypter.EncryptMd5(pwd);
            var para = string.Format("userName={0},password={1}", username, pwde);
            var parae = Encrypter.EncryptDes(para);
            var ret = WebHelper.Get(LoginUrl, "para=" + parae);
            var reto = JsonConvert.DeserializeObject<Net965808Ret>(ret);
            userid = reto.userId;
            return reto.IsOk();
        }

        public bool Regist(string username, string pwd)
        {
            var paras = new Dictionary<string, string>
            {
                {"loginId", username},
                {"password", Encrypter.EncryptMd5(pwd)},
                {"displayName", username},
                {"duty", ""},
                {"mobile", ""}
            };
            var ret = WebHelper.Get(RegisterUrl, paras);
            var reto = JsonConvert.DeserializeObject<Net965808Ret>(ret);
            return reto.IsOk();
        }

        public bool Modify(string username, string newPassword, string oldPassword, string telephone = "", string displayName = "")
        {
            var paras = new Dictionary<string, string>
            {
                {"userName", username},
                {"newPassword", Encrypter.EncryptMd5(newPassword)},
                {"oldPassword", Encrypter.EncryptMd5(oldPassword)},
                {"telephone", telephone},
                {"displayName", displayName}
            };
            var ret = WebHelper.Get(ModifyUrl, paras);
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
