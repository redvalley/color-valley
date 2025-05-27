using System.Security.Cryptography;

namespace ColorValley.Helper
{
    /// <summary>
    /// Helper class with cryptography related helper methods.
    /// </summary>
    public static class CryptoHelper
    {
        private static MD5 _md5 = MD5.Create();

        /// <summary>
        /// Gets the MD5 hash string from the specified file.
        /// </summary>
        /// <param name="md5HashFile">The MD5 hash file.</param>
        /// <returns></returns>
        public static string GetMD5HashString(string md5HashFile)
        {
            var contentFileContents = File.ReadAllBytes(md5HashFile);
            var localFileHash = _md5.ComputeHash(contentFileContents);
            return BitConverter.ToString(localFileHash);
        }

        /// <summary>
        /// Encrypts the data with aes as base64 string.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="keyBase64">The key base64.</param>
        /// <param name="ivBase64">The iv base64.</param>
        public static string EncryptDataWithAesAsBase64String(string plainText, string keyBase64, string ivBase64)
        {
            using Aes aesAlgorithm = Aes.Create();
            ICryptoTransform encryptor = aesAlgorithm.CreateEncryptor(Convert.FromBase64String(keyBase64), Convert.FromBase64String(ivBase64));

            byte[] encryptedData;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    encryptedData = memoryStream.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// Decrypts the base64 text with aes.
        /// </summary>
        /// <param name="cipherText">The cipher text.</param>
        /// <param name="keyBase64">The key base64.</param>
        /// <param name="ivBase64">The iv base64.</param>
        /// <returns>System.String.</returns>
        public static string DecryptBase64TextWithAes(string cipherText, string keyBase64, string ivBase64)
        {
            using Aes aesAlgorithm = Aes.Create();

            aesAlgorithm.Key = Convert.FromBase64String(keyBase64);
            aesAlgorithm.IV = Convert.FromBase64String(ivBase64);

            ICryptoTransform decryptor = aesAlgorithm.CreateDecryptor();

            byte[] cipher = Convert.FromBase64String(cipherText);


            using MemoryStream memoryStream = new MemoryStream(cipher);
            using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using StreamReader streamReader = new StreamReader(cryptoStream);

            return streamReader.ReadToEnd();
        }
    }
}
