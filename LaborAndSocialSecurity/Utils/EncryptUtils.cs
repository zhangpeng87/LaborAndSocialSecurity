using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace LaborAndSocialSecurity.Utils
{
    public class EncryptUtils
    {
        #region 散列运算加密。

        /// <summary>
        /// 使用MD5加密字符串。
        /// </summary>
        /// <param name="originalString">需要加密的字符串。</param>
        /// <returns>加密后的字符串。</returns>
        public static string Md5Encrypt(string originalString)
        {
            //byte[] bytes = { 0x35, 0x24, 0x76, 0x12 };
            byte[] bytes = Encoding.Default.GetBytes(originalString);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            //Console.WriteLine(sb);
            return sb.ToString();
        }

        /// <summary>
        /// (SHA256)。
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string SHA256Encrypt(string plainText)
        {
            string output = null;
            HashAlgorithm alg = HashAlgorithm.Create("SHA256");
            byte[] plainData = Encoding.UTF8.GetBytes(plainText);

            byte[] hashData = alg.ComputeHash(plainData);
            alg.Clear();
            output = BitConverter.ToString(hashData).Replace("-", string.Empty).ToLowerInvariant();

            return output;
        }

        #endregion 散列运算加密。

        #region 劳保实名制的AES加密/解密

        private static RijndaelManaged GetRijndaelManaged(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes1 = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes1, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes1.Length));

            string initVector = secretKey.Substring(0, 16);
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = secretKeyBytes1,
                IV = Encoding.UTF8.GetBytes(initVector)
            };
        }

        private static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        // Encrypts plaintext using AES 128bit key and a Chain Block Cipher and returns a base64 encoded string
        public static string Encrypt(string plainText, string key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(key)));
        }

        public static string Decrypt(string encryptedText, string key)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(key)));
        }

        #endregion 劳保实名制的AES加密/解密

        #region 品茗的AES加密算法

        private static RijndaelManaged GetRijndaelManagedPm(string secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes1 = Encoding.UTF8.GetBytes(secretKey);
            var secretKeyBytes = StrToToHexByte(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public static string EncryptPm(string plainText, string key)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            if (plainBytes.Length % 16 != 0)
            {
                var newOriginalBytes = new byte[(plainBytes.Length / 16 + 1) * 16];
                Array.Copy(plainBytes, 0, newOriginalBytes, 0, plainBytes.Length);
                plainBytes = newOriginalBytes;
            }

            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManagedPm(key)));
        }

        #endregion 品茗的AES加密算法

        #region 字符串转16进制字节数组

        /// <summary>
        /// 字符串转16进制字节数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        private static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion 字符串转16进制字节数组
    }
}