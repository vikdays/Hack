using CryptoGeneral.Services.Interfaces;

namespace CryptoGeneral.Services.Implementations;

public class RsaService : IRsaService
{
    // n = p*q; p, q - простые числа
    /* e - число (обычно простое, но необязательно), которое меньше fi(N) и является взаимно простым с fi(N)
     (не имеющих общих делителей друг с другом, кроме 1).*/
    // функция Эйлера: fi(N) = (p-1) * (q-1)
    // (e, n) - открытый ключ
    public string RsaEncrypt(string message, int n, int e)
    {
        string result = "";

        foreach (var symbol in message)
        {
            int encryptedSymbol = symbol; 

            for (int i = 1; i < e; i++)
                encryptedSymbol = (encryptedSymbol * symbol) % n; // C = M^e  mod n, C - зашифрованный символ, М - исходный символ

            result += (char)encryptedSymbol;
        }

        return result;
    }
    
    /* число d, обратное числу e по модулю fi(N) .Т.е. остаток от деления (d*e) и fi(N) должен быть равен 1.
     e *d (mod fi(n)) = 1 */
    // (d, n) - закрытый(секретный) ключ
    public string RsaDecrypt(string message, int n, int d)
    {
        string result = "";

        foreach (var symbol in message)
        {
            int decryptedSymbol = symbol;
            
            for (int i = 1; i < d; i++)
                decryptedSymbol = (decryptedSymbol * symbol) % n; // M = C^d  mod n

            result += (char)decryptedSymbol;
        }

        return result;
    }
    
    // Взлом шифра RSA
    public string RsaHack(string message, int n, int e)
    {
        string previousResult;
        string currentResult = message;

        do
        {
            previousResult = currentResult;
            string newResult = "";
            
            foreach (var symbol in currentResult)
            {
                int encryptedSymbol = symbol;

                for (int i = 1; i < e; i++)
                    encryptedSymbol = encryptedSymbol * symbol % n; // C = M^e  mod n

                newResult += (char)encryptedSymbol;
            }

            currentResult = newResult;
        } while (currentResult != message);

        return previousResult;
    }
}