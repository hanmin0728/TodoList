using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
public static class CryptoUtility 
{
    private static readonly byte[] Key;
    private static readonly byte[] IV;
    static CryptoUtility()
    {
        // "아무 문자열"이나 넣어도 SHA256을 거치면 무조건 32바이트가 됩니다.
        string secretKey = "SpectorOffice_Ghost_Worker_Secret_Key_2026";
        string secretIV = "SpectorOffice_IV_Static";

        using (SHA256 sha256 = SHA256.Create())
        {
            Key = sha256.ComputeHash(Encoding.UTF8.GetBytes(secretKey));

            // IV는 16바이트여야 하므로 해시값의 절반만 딱 잘라서 씁니다.
            byte[] fullHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(secretIV));
            IV = new byte[16];
            Array.Copy(fullHash, IV, 16);
        }
    }

    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }
    public static string Decrypt(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }
}
