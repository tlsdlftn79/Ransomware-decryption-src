using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AesExample
{
    public static void Main()
    {
        string targetDirectory = @"C:\";
        string[] fileEntries = Directory.GetFiles(targetDirectory);

        // 비밀번호와 솔트 값을 설정하세요.
        string password = "비밀번호 설정";
        string salt = "16";

        // Rfc2898DeriveBytes 객체 생성
        var keyGenerator = new Rfc2898DeriveBytes(password, Encoding.UTF8.GetBytes(salt));

        // 32 bytes for AES-256
        byte[] keyArray = keyGenerator.GetBytes(32);

        foreach (string fileName in fileEntries)
        {
            if (Path.GetExtension(fileName) == ".enc")
            {
                DecryptFile(fileName, keyArray);
            }
        }
    }

    static void DecryptFile(string inputFile, byte[] keyArray)
    {
        try
        {
            using (AesManaged aes = new AesManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;
                aes.Key = keyArray;

                ICryptoTransform transform = aes.CreateDecryptor();

                using (FileStream fsInput =
                    new FileStream(inputFile, FileMode.Open))
                using (FileStream fsDecrypted =
                    new FileStream(inputFile.Replace(".enc", ""), FileMode.Create))
                using (CryptoStream cryptostream =
                    new CryptoStream(fsInput, transform, CryptoStreamMode.Read))
                    cryptostream.CopyTo(fsDecrypted);

            }

            File.Delete(inputFile);

            Console.WriteLine("Decryption done for file: " + inputFile);

        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: {0}", e.Message);
            return;
        }
    }
}
