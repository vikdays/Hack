namespace CryptoGeneral.Services.Interfaces;

public interface IRsaService
{
    string RsaEncrypt(string message, int n, int e);
    string RsaDecrypt(string message, int n, int d);
    string RsaHack(string message, int n, int e);
}