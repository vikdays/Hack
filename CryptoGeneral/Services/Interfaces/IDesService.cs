namespace CryptoGeneral.Services.Interfaces;

public interface IDesService
{
    string Encrypt(string message, string key);
    string Decrypt(string message, string key);
    string BruteForceDecrypt(string ciphertext, string knownPlaintext);
}