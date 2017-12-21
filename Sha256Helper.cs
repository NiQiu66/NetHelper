using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.ThirdParty.Server.Util
{
    public class Sha256Helper
    {
        /// <summary>
        /// 根据秘钥签名
        /// </summary>
        /// <param name="data">签名参数</param>
        /// <param name="secureKey">秘钥</param>
        /// <returns></returns>
        public static string SignBySecureKey(Dictionary<string, string> data, String secureKey)
        {
            data = DicHelper.FilterBlank(data);
            data = data.OrderBy(s => s.Key).ToDictionary(d => d.Key, d => d.Value);  //按照key排序         
            var stringData = DicHelper.ConverDicToStr(data, '=', '&');
            String strBeforeSha256 = $"{stringData}&{Sha256(secureKey)}";
            return Sha256(strBeforeSha256);
        }
        /// <summary>
        /// 计算data 的SHA256哈希值
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Sha256(string data)
        {
            SHA256 mySHA256 = SHA256.Create();
            byte[] source = Encoding.UTF8.GetBytes(data);
            byte[] crypto = mySHA256.ComputeHash(source);
            return ByteToHexStr(crypto).ToLower();
        }
        /// <summary> 
        /// 字节数组转16进制字符串 
        /// </summary> 
        /// <param name="bytes"></param> 
        /// <returns></returns> 
        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";

            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }

            return returnStr;
        }
    }
}
