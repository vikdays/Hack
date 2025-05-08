using System.Security.Cryptography;
using System.Text;
using CryptoGeneral.Services.Interfaces;

namespace CryptoGeneral.Services.Implementations;

public class Md5Service : IMd5Service
{
    private static readonly Random random = new Random();
    
    // Основной метод хеширования (из второго примера)
    public string GetHash(string message)
    {
        // Используем стандартную реализацию MD5 для простоты
        byte[] hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(message));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    // Хэширование документа с 16-ричными данными (улучшенная версия)
    public string GetDocumentHash(string document)
    {
        if (string.IsNullOrEmpty(document) || document.Length % 2 != 0)
            throw new ArgumentException("Document must be non-empty and have even length");

        byte[] messageBytes = new byte[document.Length / 2];
        
        for (int i = 0; i < document.Length; i += 2)
        {
            byte firstNibble = HexCharToByte(document[i]);
            byte secondNibble = HexCharToByte(document[i+1]);
            messageBytes[i/2] = (byte)((firstNibble << 4) | secondNibble);
        }
        
        byte[] hash = MD5.Create().ComputeHash(messageBytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    // Поиск коллизий (комбинированный подход)
    public (string FirstMessage, string SecondMessage, string CommonHash)? FindCollision(int maxAttempts = 1_000_000)
    {
        var hashes = new Dictionary<string, string>();
        
        for (int i = 0; i < maxAttempts; i++)
        {
            string randomMessage = GenerateRandomHexString();
            string hash = GetDocumentHash(randomMessage);

            if (hashes.TryGetValue(hash, out var existingMessage) && existingMessage != randomMessage)
            {
                return (existingMessage, randomMessage, hash);
            }
            
            hashes[hash] = randomMessage;
        }

        return null;
    }

    // Вспомогательные методы
    private byte HexCharToByte(char c)
    {
        return c switch
        {
            >= '0' and <= '9' => (byte)(c - '0'),
            >= 'a' and <= 'f' => (byte)(10 + (c - 'a')),
            >= 'A' and <= 'F' => (byte)(10 + (c - 'A')),
            _ => throw new ArgumentException($"Invalid hex character: {c}")
        };
    }

    private string GenerateRandomHexString(int minLength = 64, int maxLength = 128)
    {
        int length = random.Next(minLength/2, maxLength/2) * 2; // Гарантируем четную длину
        var sb = new StringBuilder(length);
        
        for (int i = 0; i < length; i++)
        {
            sb.Append("0123456789abcdef"[random.Next(16)]);
        }
        
        return sb.ToString();
    }
    
    
}