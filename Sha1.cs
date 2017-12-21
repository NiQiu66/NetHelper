using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.ThirdParty.Server.Util
{
    class Sha1
    {
        /// <summary>
        /// 生成SHA_1签名
        /// </summary>
        /// <param name="supplierKey"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <returns></returns>
        public static String Sign(String[] arr)
        {
            //String[] arr = new String[] { supplierKey, timestamp, nonce };
            // 将supplierKey、timestamp、nonce、三个参数进行字典序排序
            Array.Sort(arr);
            var content = new StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                content.Append(arr[i]);
            }
            String signature = null;
            byte[] key = Encoding.Default.GetBytes(content.ToString());

            SHA1 sha1 = SHA1Managed.Create();

            byte[] digest = sha1.ComputeHash(key);
            signature = ByteToStr(digest);

            return signature;
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        private static String ByteToStr(byte[] byteArray)
        {
            String strDigest = "";
            for (int i = 0; i < byteArray.Length; i++)
            {
                strDigest += ByteToHexStr(byteArray[i]);
            }
            return strDigest;
        }
        /// <summary>
        /// 将字节转换为十六进制字符串
        /// </summary>
        /// <param name="mByte"></param>
        /// <returns></returns>
        private static String ByteToHexStr(byte mByte)
        {

            char[] Digit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            char[] tempArr = new char[2];
            tempArr[0] = Digit[(mByte >> 4) & 0X0F];
            tempArr[1] = Digit[mByte & 0X0F];

            String s = new String(tempArr);
            return s;
        }
    }
}
