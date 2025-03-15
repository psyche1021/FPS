using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SimpleEncryptionUtility
{
    // AES 암호화를 위한 고정된 키 (32바이트, 256비트)
    private static readonly string key = "12345678901234567890123456789012"; // 32바이트

    // AES 암호화 메서드 (매번 새로운 IV 생성)
    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // 새로운 IV 생성
            aes.GenerateIV();
            byte[] ivBytes = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, ivBytes);
            using (MemoryStream ms = new MemoryStream())
            {
                // IV를 먼저 메모리 스트림에 기록
                ms.Write(ivBytes, 0, ivBytes.Length);

                // 평문을 암호화하여 메모리 스트림에 기록
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    // IV와 암호문이 함께 포함된 결과를 반환
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    // AES 복호화 메서드 (저장된 IV 사용)
    public static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // 암호문에서 IV 추출 (처음 16바이트)
            byte[] ivBytes = new byte[16];
            Array.Copy(fullCipher, 0, ivBytes, 0, ivBytes.Length);

            // 나머지 부분은 암호화된 데이터
            byte[] cipherBytes = new byte[fullCipher.Length - ivBytes.Length];
            Array.Copy(fullCipher, ivBytes.Length, cipherBytes, 0, cipherBytes.Length);

            aes.IV = ivBytes;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(cipherBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}