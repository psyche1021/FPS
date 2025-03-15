using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public static class SimpleEncryptionUtility
{
    // AES ��ȣȭ�� ���� ������ Ű (32����Ʈ, 256��Ʈ)
    private static readonly string key = "12345678901234567890123456789012"; // 32����Ʈ

    // AES ��ȣȭ �޼��� (�Ź� ���ο� IV ����)
    public static string Encrypt(string plainText)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // ���ο� IV ����
            aes.GenerateIV();
            byte[] ivBytes = aes.IV;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, ivBytes);
            using (MemoryStream ms = new MemoryStream())
            {
                // IV�� ���� �޸� ��Ʈ���� ���
                ms.Write(ivBytes, 0, ivBytes.Length);

                // ���� ��ȣȭ�Ͽ� �޸� ��Ʈ���� ���
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();

                    // IV�� ��ȣ���� �Բ� ���Ե� ����� ��ȯ
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    // AES ��ȣȭ �޼��� (����� IV ���)
    public static string Decrypt(string cipherText)
    {
        byte[] fullCipher = Convert.FromBase64String(cipherText);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            // ��ȣ������ IV ���� (ó�� 16����Ʈ)
            byte[] ivBytes = new byte[16];
            Array.Copy(fullCipher, 0, ivBytes, 0, ivBytes.Length);

            // ������ �κ��� ��ȣȭ�� ������
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