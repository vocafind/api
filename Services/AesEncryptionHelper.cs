namespace vocafind_api.Services;
using System.Security.Cryptography;
using System.Text;

public class AesEncryptionHelper
{
    private readonly string _encryptionKey;

    public AesEncryptionHelper(IConfiguration configuration)
    {
        // Ambil key dari appsettings.json
        _encryptionKey = configuration["Encryption:AesKey"]
            ?? throw new InvalidOperationException("Encryption key tidak ditemukan di appsettings.json");

        // Validasi key harus 32 karakter (256-bit)
        if (_encryptionKey.Length != 32)
            throw new InvalidOperationException("Encryption key harus tepat 32 karakter untuk AES-256");
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                // Simpan IV di awal stream
                msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        byte[] buffer = Convert.FromBase64String(cipherText);
        byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;

            // Ambil IV dari awal buffer
            byte[] iv = new byte[aes.IV.Length];
            Array.Copy(buffer, 0, iv, 0, iv.Length);
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            {
                return srDecrypt.ReadToEnd();
            }
        }
    }

    // Untuk enkripsi file
    public async Task EncryptFileAsync(string inputPath, string outputPath)
    {
        byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV();

            using (FileStream fsOutput = new FileStream(outputPath, FileMode.Create))
            {
                // Tulis IV di awal file
                await fsOutput.WriteAsync(aes.IV, 0, aes.IV.Length);

                using (CryptoStream cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (FileStream fsInput = new FileStream(inputPath, FileMode.Open))
                {
                    await fsInput.CopyToAsync(cs);
                }
            }
        }
    }

    // Untuk dekripsi file
    public async Task DecryptFileAsync(string inputPath, string outputPath)
    {
        byte[] key = Encoding.UTF8.GetBytes(_encryptionKey);

        using (Aes aes = Aes.Create())
        {
            aes.Key = key;

            using (FileStream fsInput = new FileStream(inputPath, FileMode.Open))
            {
                // Baca IV dari awal file
                byte[] iv = new byte[16]; // AES IV selalu 16 bytes
                await fsInput.ReadAsync(iv, 0, iv.Length);
                aes.IV = iv;

                using (CryptoStream cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (FileStream fsOutput = new FileStream(outputPath, FileMode.Create))
                {
                    await cs.CopyToAsync(fsOutput);
                }
            }
        }
    }
}