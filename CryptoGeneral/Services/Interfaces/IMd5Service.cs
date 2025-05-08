namespace CryptoGeneral.Services.Interfaces;

public interface IMd5Service
{
    string GetHash(string message);
    string GetDocumentHash(string document);
    (string FirstMessage, string SecondMessage, string CommonHash)? FindCollision(int maxAttempts = 1_000_000);
}