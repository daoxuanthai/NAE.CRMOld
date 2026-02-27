using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace tcs.lib
{
    public static class CryptoHelper
    {
        /// <summary>
        /// Tính toán SHA1 của một chuỗi bất kỳ
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string CalculateSHA1(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            return hash.ToLower();
        }

        public static string CalculateSHA256(string text)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            var sha = SHA256.Create();
            byte[] outputBytes = sha.ComputeHash(byteArray);
            return BitConverter.ToString(outputBytes).Replace("-", "");
        }

        public static string MD5(string text)
        {
            var byteArray = Encoding.UTF8.GetBytes(text);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(byteArray);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString().ToUpper();
        }

        public static string HashHMAC256String(string keyHex, string message)
        {
            byte[] hash = HashHMAC256(Encoding.UTF8.GetBytes(keyHex), Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string HashHMAC256StringPHP(string keyHex, string message)
        {
            if (string.IsNullOrEmpty(keyHex) || string.IsNullOrEmpty(message))
                return string.Empty;

            Encoding encoding = Encoding.UTF8;
            var keyByte = encoding.GetBytes(keyHex);
            var hmacsha256 = new HMACSHA256(keyByte);
            hmacsha256.ComputeHash(encoding.GetBytes(message));

            return ByteToString(hmacsha256.Hash);
        }

        public static byte[] HashHMAC256(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        public static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary;
        }

        /// <summary>
        ///     Gets a MD5 hash of the current instance.
        /// </summary>
        /// <param name="instance">
        ///     The instance being extended.
        /// </param>
        /// <returns>
        ///     A base 64 encoded string representation of the hash.
        /// </returns>
        public static string GetMD5Hash(this object instance)
        {
            return instance.GetHash<MD5CryptoServiceProvider>();
        }

        /// <summary>
        /// Gets a hash of the current instance.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the Cryptographic Service Provider to use.
        /// </typeparam>
        /// <param name="instance">
        /// The instance being extended.
        /// </param>
        /// <returns>
        /// A base 64 encoded string representation of the hash.
        /// </returns>
        public static string GetHash<T>(this object instance) where T : HashAlgorithm, new()
        {
            T cryptoServiceProvider = new T();
            return ComputeHash(instance, cryptoServiceProvider);
        }

        private static string ComputeHash<T>(object instance, T cryptoServiceProvider) where T : HashAlgorithm, new()
        {
            DataContractSerializer serializer = new DataContractSerializer(instance.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, instance);
                cryptoServiceProvider.ComputeHash(memoryStream.ToArray());
                return Convert.ToBase64String(cryptoServiceProvider.Hash);
            }
        }
        /// <summary>
        /// Converts to hexadecimal string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ConvertToHexString(long value)
        {
            var valueBytes = Encoding.ASCII.GetBytes(value.ToString(CultureInfo.InvariantCulture));
            return BitConverter.ToString(valueBytes).Replace("-", "");
        }

        public static long ConvertHexStringToInt(string hexString)
        {
            int numberChars = hexString.Length;
            var bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return Convert.ToInt64(Encoding.ASCII.GetString(bytes));
        }

        /// <summary>
        /// Sinh mã xác thực
        /// </summary>
        /// <param name="codelen"></param>
        /// <returns></returns>
        public static string GenerateVerificationCode(int codelen)
        {
            string code = "";
            var rnd = new Random();
            for (int i = 0; i < codelen; i++)
            {
                code += rnd.Next(10);
            }
            return code;
        }

        /// <summary>
        /// Mã hóa với RSA
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="RSAKey"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        public static byte[] RSAEncryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    encryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
#if DEBUG
                throw e;
#endif
                return null;
            }
        }

        /// <summary>
        /// Giải mã với RSA
        /// </summary>
        /// <param name="Data"></param>
        /// <param name="RSAKey"></param>
        /// <param name="DoOAEPPadding"></param>
        /// <returns></returns>
        public static byte[] RSADecryption(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    decryptedData = RSA.Decrypt(Data, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
#if DEBUG
                throw e;
#endif
                return null;
            }
        }

        /// <summary>
        /// Tạo ngẫu nhiên một mật khẩu
        /// </summary>
        /// <param name="type">
        /// <para>0: Mật khẩu chỉ bao gồm số</para>
        /// <para>1: Mật khẩu bao gồm ký tự và số</para>
        /// </param>
        /// <param name="length">Độ dài mật khẩu</param>
        /// <returns></returns>
        public static string GeneratePassword(int type = 0, int length = 4)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;
            if (type == 1)
            {
                characters += alphabets + small_alphabets + numbers;
            }
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character, StringComparison.Ordinal) != -1);
                otp += character;
            }
            return otp;
        }

        public static string EncryptCustomer(string phone, string password, string privateKey)
        {
            // Dữ liệu sẽ mã hóa
            string dataString = phone + password;

            // Mã hóa - tạo token            
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSA.FromXmlString(privateKey);
            var data = Encoding.UTF8.GetBytes(dataString);
            var encryptedData = RSAEncryption(data, RSA.ExportParameters(false), false);

            // Token dùng xác thực khách hàng
            return encryptedData.ToHexString();
        }

        public static string EncryptCustomer(string phone, string password, out string privateKey)
        {
            // Dữ liệu sẽ mã hóa
            string dataString = phone + password + DateTime.Now.ToOADate() + "rc3YvXPfpZyJWJEFEtuUDBJv_VuiVui";

            // Mã hóa - tạo token            
            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            var data = Encoding.UTF8.GetBytes(dataString);
            var encryptedData = RSAEncryption(data, RSA.ExportParameters(false), false);

            // Lưu lại RSA-PrivateKey để giải mã
            privateKey = RSA.ToXmlString(true);

            // Token dùng xác thực khách hàng
            return encryptedData.ToHexString();
        }

        public static string SimplifySearchKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return null;
            keyword =
                Regex.Replace(keyword,
                    @"[^a-z0-9A-ZÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀẾỂưăạảấầẩẫậắằẳẵặẹẻẽềếểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵýỷỹ& \.\-_ ]+",
                    "").Replace("  ", " ").Replace("  ", " ").Trim();
            if (keyword.Length > 50)
                keyword = keyword.Substring(0, 50);
            return keyword;
        }
        
        public static string Encrypt(string text, string key)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateEncryptor())
                    {
                        byte[] textBytes = Encoding.UTF8.GetBytes(text);
                        byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
                        return Convert.ToBase64String(bytes, 0, bytes.Length);
                    }
                }
            }
        }

        public static string Decrypt(string cipher, string key)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                using (var tdes = new TripleDESCryptoServiceProvider())
                {
                    tdes.Key = md5.ComputeHash(Encoding.UTF8.GetBytes(key));
                    tdes.Mode = CipherMode.ECB;
                    tdes.Padding = PaddingMode.PKCS7;

                    using (var transform = tdes.CreateDecryptor())
                    {
                        byte[] cipherBytes = Convert.FromBase64String(cipher);
                        byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
                        return Encoding.UTF8.GetString(bytes);
                    }
                }
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

    }
}
