using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tuhu.Service.ThirdParty.Server.Util
{
    public class RSAHelper
    {
        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="content">密文</param>
        /// <param name="secretKey">私钥</param>
        /// <param name="charset">编码方式</param>
        /// <returns></returns>
        public static string RSADecrypt(string content, string secretKey, string charset = "UTF-8")
        {
            string result = string.Empty;
            var base64Key = Convert.FromBase64String(secretKey);
            var provider = GetRSACryptoServiceProvider(base64Key);          
            byte[] data = Convert.FromBase64String(content);
            int maxBlockSize = provider.KeySize / 8; //解密块最大长度限制
            if (data.Length <= maxBlockSize)//如果解密数据较短，则直接解密
            {
                byte[] cipherbytes = provider.Decrypt(data, false);
                result = Encoding.GetEncoding(charset).GetString(cipherbytes);
            }
            else                            //否则分块解密
            {
                MemoryStream crypStream = new MemoryStream(data);
                MemoryStream plaiStream = new MemoryStream();
                Byte[] buffer = new Byte[maxBlockSize];
                int blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                while (blockSize > 0)
                {
                    Byte[] toDecrypt = new Byte[blockSize];
                    Array.Copy(buffer, 0, toDecrypt, 0, blockSize);
                    Byte[] cryptograph = provider.Decrypt(toDecrypt, false);
                    plaiStream.Write(cryptograph, 0, cryptograph.Length);
                    blockSize = crypStream.Read(buffer, 0, maxBlockSize);
                }
                result = Encoding.GetEncoding(charset).GetString(plaiStream.ToArray());
            }

            return result;
        }
        /// <summary>
        /// 根据key初始化解密RSACryptoServiceProvider参数
        /// </summary>
        /// <param name="privkey"></param>
        /// <returns></returns>
        public static RSACryptoServiceProvider GetRSACryptoServiceProvider(byte[] privkey)
        {
            byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

            // --------- Set up stream to decode the asn.1 encoded RSA private key ------
            MemoryStream mem = new MemoryStream(privkey);
            BinaryReader binr = new BinaryReader(mem);  //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;
            int elems = 0;
            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
            {
                binr.ReadByte();    //advance 1 byte
            }
            else if (twobytes == 0x8230)
            {
                binr.ReadInt16();    //advance 2 bytes
            }
            else
            {
                return null;
            }
            twobytes = binr.ReadUInt16();
            if (twobytes != 0x0102) //version number
                return null;
            bt = binr.ReadByte();
            if (bt != 0x00)
                return null;
            //------ all private key components are Integer sequences ----
            elems = GetIntegerSize(binr);
            MODULUS = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            E = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            D = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            P = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            Q = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DP = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DQ = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            IQ = binr.ReadBytes(elems);
            // ------- create RSACryptoServiceProvider instance and initialize with public key -----
            CspParameters CspParameters = new CspParameters();
            CspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
            int bitLen = 1024;
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(bitLen, CspParameters);
            RSAParameters RSAparams = new RSAParameters();
            RSAparams.Modulus = MODULUS;
            RSAparams.Exponent = E;
            RSAparams.D = D;
            RSAparams.P = P;
            RSAparams.Q = Q;
            RSAparams.DP = DP;
            RSAparams.DQ = DQ;
            RSAparams.InverseQ = IQ;
            RSA.ImportParameters(RSAparams);

            return RSA;
        }
        /// <summary>
        /// 获取流的大小
        /// </summary>
        /// <param name="binr"></param>
        /// <returns></returns>
        private static int GetIntegerSize(BinaryReader binr)
        {
            byte bt = 0;
            byte lowbyte = 0x00;
            byte highbyte = 0x00;
            int count = 0;
            bt = binr.ReadByte();
            if (bt != 0x02)     //expect integer
                return 0;
            bt = binr.ReadByte();

            if (bt == 0x81)
            {
                count = binr.ReadByte();    // data size in next byte
            }
            else if (bt == 0x82)
            {
                highbyte = binr.ReadByte(); // data size in next 2 bytes
                lowbyte = binr.ReadByte();
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
                count = BitConverter.ToInt32(modint, 0);
            }
            else
            {
                count = bt;     // we already have the data size
            }

            while (binr.ReadByte() == 0x00)
            {   //remove high order zeros in data
                count -= 1;
            }

            binr.BaseStream.Seek(-1, SeekOrigin.Current);       //last ReadByte wasn't a removed zero, so back up a byte

            return count;
        }
    }
}
