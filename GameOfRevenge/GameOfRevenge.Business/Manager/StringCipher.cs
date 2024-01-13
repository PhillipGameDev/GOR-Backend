using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class StringCipher
{
    public static string Encrypt(string plainText, string passPhrase)
    {
        byte[] secret = Encoding.UTF8.GetBytes(plainText);
        byte[] key = Encoding.UTF8.GetBytes(passPhrase);
        using (MemoryStream ms = new MemoryStream())
        {
            using (AesManaged cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.PKCS7;
                cryptor.KeySize = 128;
                cryptor.BlockSize = 128;

                byte[] iv = cryptor.IV;

                using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(secret, 0, secret.Length);
                }
                byte[] encryptedContent = ms.ToArray();

                byte[] result = new byte[iv.Length + encryptedContent.Length];

                Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                Buffer.BlockCopy(encryptedContent, 0, result, iv.Length, encryptedContent.Length);

                return Convert.ToBase64String(result);
            }
        }
    }

    public static string Decrypt(string text, string passPhrase)
    {
        byte[] secret = Convert.FromBase64String(text);
        byte[] key = Encoding.UTF8.GetBytes(passPhrase);
        byte[] iv = new byte[16];
        byte[] encryptedContent = new byte[secret.Length - 16];

        Buffer.BlockCopy(secret, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(secret, iv.Length, encryptedContent, 0, encryptedContent.Length);

        using (MemoryStream ms = new MemoryStream())
        {
            using (AesManaged cryptor = new AesManaged())
            {
                cryptor.Mode = CipherMode.CBC;
                cryptor.Padding = PaddingMode.PKCS7;
                cryptor.KeySize = 128;
                cryptor.BlockSize = 128;

                using (CryptoStream cs = new CryptoStream(ms, cryptor.CreateDecryptor(key, iv), CryptoStreamMode.Write))
                {
                    cs.Write(encryptedContent, 0, encryptedContent.Length);
                }
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
    }
}