namespace CryptoGeneral.Data;

public class MergeEncodeDto
{
    public string Message { get; set; }
    public string DesKey { get; set; }
    public string KnownPlaintext { get; set; }
    public int n { get; set; }
    public int e { get; set; }
}