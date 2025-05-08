using CryptoGeneral.Services.Interfaces;

namespace CryptoGeneral.Services.Implementations;
#pragma warning disable 1591
public class DesService : IDesService
{
    private const int BlockSize = 64;       
    private const int BinaryCharLength = 8;  
    private const int Rounds = 16;

    public string Encrypt(string message, string key)
    {
        message = PadMessage(message);
        List<string> blocks = GetBinaryBlocks(message);
        string binKey = GetBinaryKey(key);

        for (int r = 0; r < Rounds; r++)
        {
            for (int i = 0; i < blocks.Count; i++)
                blocks[i] = FeistelEncrypt(blocks[i], binKey);

            binKey = ShiftLeft(binKey, 1);
        }

        var binaryOutput = string.Concat(blocks.Select(GetStringFromBinary));
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(binaryOutput));

    }

    public string Decrypt(string message, string key)
    {
        message = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(message)); 
        message = PadMessage(message);  
        List<string> blocks = GetBinaryBlocks(message);  // Разбиение на блоки
        string binKey = GetBinaryKey(key);  // Преобразование ключа в бинарный формат

        for (int i = 0; i < Rounds - 1; i++)
            binKey = ShiftLeft(binKey, 1);  // Сдвиг ключа

        for (int r = 0; r < Rounds; r++)
        {
            for (int i = 0; i < blocks.Count; i++)
                blocks[i] = FeistelDecrypt(blocks[i], binKey);  // Дешифрование каждого блока

            binKey = ShiftRight(binKey, 1);  // Сдвиг ключа обратно
        }

        return string.Concat(blocks.Select(GetStringFromBinary)).TrimEnd();  
    }

    
    public string BruteForceDecrypt(string ciphertext, string knownPlaintext)
    {
        // Декодируем ciphertext из Base64
        string decodedCipher = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(ciphertext));
    
        // Перебираем все возможные 16-битные ключи 
        for (int key = 0; key <= 65535; key++)
        {
            // Преобразуем ключ в строку из 16 нулей и единиц
            string trialKey = Convert.ToString(key, 2).PadLeft(16, '0');
        
            try
            {
                string decrypted = Decrypt(ciphertext, trialKey).TrimEnd();
            
                if (decrypted.StartsWith(knownPlaintext))
                {
                    return trialKey;
                }
            }
            catch
            {
                continue;
            }
        }

        return "Key not found";
    }


    private string FeistelEncrypt(string block, string key)
    {
        string L = block.Substring(0, 32);
        string R = block.Substring(32, 32);
        string newL = R;
        string newR = Xor(L, SimpleF(R, key));
        return newL + newR;
    }

    private string FeistelDecrypt(string block, string key)
    {
        string L = block.Substring(0, 32);
        string R = block.Substring(32, 32);
        string newR = L;
        string newL = Xor(R, SimpleF(L, key));
        return newL + newR;
    }

    private string SimpleF(string halfBlock, string key)
    {
        return Xor(halfBlock, key.Substring(0, 32));
    }

    private string GetBinaryKey(string key)
    {
        return key.PadRight(64, '0'); 
    }

    private List<string> GetBinaryBlocks(string text)
    {
        var result = new List<string>();
        for (int i = 0; i < text.Length; i += 8)
        {
            string block = text.Substring(i, 8);
            string binary = string.Concat(block.Select(c => Convert.ToString(c, 2).PadLeft(BinaryCharLength, '0')));
            result.Add(binary);
        }
        return result;
    }

    private string GetStringFromBinary(string binary)
    {
        var chars = new List<char>();
        for (int i = 0; i < binary.Length; i += 8)
            chars.Add((char)Convert.ToInt32(binary.Substring(i, 8), 2));
        return new string(chars.ToArray());
    }

    private string PadMessage(string text)
    {
        int padLen = 8 - (text.Length % 8);
        return text + new string(' ', padLen == 8 ? 0 : padLen);
    }

    private string Xor(string a, string b)
    {
        return string.Concat(a.Zip(b, (x, y) => x == y ? '0' : '1'));
    }

    private string ShiftLeft(string s, int count)
    {
        return s.Substring(count) + s.Substring(0, count);
    }

    private string ShiftRight(string s, int count)
    {
        return s.Substring(s.Length - count) + s.Substring(0, s.Length - count);
    }
}
