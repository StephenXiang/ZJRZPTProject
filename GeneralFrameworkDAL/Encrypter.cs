using System;
using System.Security.Cryptography;
using System.Text;

namespace GeneralFrameworkDAL
{
    /// <summary>
    /// 加密/解密类
    /// </summary>
    public class Encrypter
    {
        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public static string EncryptDes(string pToEncrypt, string sKey)
        {
            using (var des = new DESCryptoServiceProvider())
            {
                var inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = Encoding.UTF8.GetBytes(sKey);
                des.IV = Encoding.UTF8.GetBytes(sKey);
                var ms = new System.IO.MemoryStream();
                using (var cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                var str = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return str;
            }
        }


        // <summary>
        // 进行DES解密。
        // </summary>
        // <param name="pToDecrypt">要解密的以Base64</param>
        // <param name="sKey">密钥，且必须为8位。</param>
        // <returns>已解密的字符串。</returns>
        public static string DecryptDes(string pToDecrypt, string sKey)
        {
            var inputByteArray = Convert.FromBase64String(pToDecrypt);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Key = Encoding.ASCII.GetBytes(sKey);
                des.IV = Encoding.ASCII.GetBytes(sKey);
                var ms = new System.IO.MemoryStream();
                using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                var str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }

        public static string EncryptMd5(string pwd)
        {
            var bytes = Encoding.Default.GetBytes(pwd);
            var md5 = new MD5CryptoServiceProvider();
            var res = md5.ComputeHash(bytes);
            return BitConverter.ToString(res).Replace("-", "").ToLower();
        }
    }
}
