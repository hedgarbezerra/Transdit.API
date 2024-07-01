using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;

namespace Transdit.Utilities.Security
{
    [ExcludeFromCodeCoverage]
    public class Cryptography : ICryptography
    {
        private readonly byte[] _key;

        public Cryptography(AppConfiguration configuration)
        {

            _key = Encoding.ASCII.GetBytes(configuration.CryptoKey);
        }
        public string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var ivAndCipherText = Convert.FromBase64String(value);
            using var aes = Aes.Create();
            var ivLength = aes.BlockSize / 8;
            aes.IV = ivAndCipherText.Take(ivLength).ToArray();
            aes.Key = _key;
            using var cipher = aes.CreateDecryptor();
            var cipherText = ivAndCipherText.Skip(ivLength).ToArray();
            var text = cipher.TransformFinalBlock(cipherText, 0, cipherText.Length);

            return Encoding.UTF8.GetString(text);
        }

        public string Encrypt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            using var aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            using var cipher = aes.CreateEncryptor();
            var text = Encoding.UTF8.GetBytes(value);
            var cipherText = cipher.TransformFinalBlock(text, 0, text.Length);

            return Convert.ToBase64String(aes.IV.Concat(cipherText).ToArray());
        }
    }
}
