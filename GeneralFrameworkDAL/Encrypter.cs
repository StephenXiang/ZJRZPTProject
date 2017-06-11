using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GeneralFrameworkDAL
{
    /// <summary>
    /// 加密/解密类
    /// </summary>
    public class Encrypter
    {
        private const string DesKey = @"h1y2i3j4l8";

        /// <summary>
        /// 进行DES加密。
        /// </summary>
        /// <param name="pToEncrypt">要加密的字符串。</param>
        /// <param name="sKey">密钥，且必须为8位。</param>
        /// <returns>以Base64格式返回的加密字符串。</returns>
        public static string EncryptDes(string pToEncrypt, string sKey = null)
        {
            using (var des = new DESCryptoServiceProvider())
            {
                if (string.IsNullOrEmpty(sKey)) sKey = DesKey;
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

        //默认密钥向量
        private static byte[] Keys = { 0x13, 0x24, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /**//// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDes2(string encryptString, string encryptKey = DesKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>  
        /// 创建Key  
        /// </summary>  
        /// <returns></returns>  
        public static string GenerateKey(string str)
        {
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DES.Create(str);
            return Encoding.ASCII.GetString(desCrypto.Key);
        }

        // <summary>
        // 进行DES解密。
        // </summary>
        // <param name="pToDecrypt">要解密的以Base64</param>
        // <param name="sKey">密钥，且必须为8位。</param>
        // <returns>已解密的字符串。</returns>
        public static string DecryptDes(string pToDecrypt, string sKey = null)
        {
            if (string.IsNullOrEmpty(sKey)) sKey = DesKey;
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
