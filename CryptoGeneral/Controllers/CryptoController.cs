using CryptoGeneral.Data;
using CryptoGeneral.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1591
// ReSharper disable All

namespace CryptoGeneral.Controllers;

[ApiController]
[Route("crypto")]
public class CryptoController : ControllerBase
{
    private readonly IDesService _desService;
    private readonly IRsaService _rsaService;
    private readonly IMd5Service _md5Service;

    public CryptoController(IDesService desService, IRsaService rsaService, IMd5Service md5Service)
    {
        _desService = desService;
        _rsaService = rsaService;
        _md5Service = md5Service;
    }
    
    // Шифрование с использованием DES
    [HttpPost]
    [Route("DES_encrypt")]
    public string DesEncrypt(DesMessage desMessage)
    {
        return _desService.Encrypt(desMessage.Text, desMessage.Key);
    }
    
    // Дешифрование с использованием DES
    [HttpPost]
    [Route("DES_decrypt")]
    public string DesDecrypt(DesMessage desMessage)
    {
        return _desService.Decrypt(desMessage.Text, desMessage.Key);
    }
    
    // Взлом DES методом brute-force
    [HttpPost]
    [Route("DES_bruteforce")]
    public string DesBruteForce([FromBody] DesBruteForceRequest request)
    {
        return _desService.BruteForceDecrypt(request.CipherText, request.KnownPlainText);
    }

    
    // Шифрование с использованием RSA
    // n = 559
    // e = 65
    [HttpPost]
    [Route("RSA_encrypt")]
    public string RsaEncrypt(RsaEncryptMessage encryptMessage)
    {
        return _rsaService.RsaEncrypt(encryptMessage.Text, encryptMessage.N, encryptMessage.E);
    }
    
    // Дешифрование с использованием RSA
    // n = 559
    // d = 473
    [HttpPost]
    [Route("RSA_decrypt")]
    public string RsaDecrypt(RsaDecryptMessage decryptMessage)
    {
        return _rsaService.RsaDecrypt(decryptMessage.Text, decryptMessage.N, decryptMessage.D);
    }

    // Взлом шифра RSA
    [HttpPost]
    [Route("RSA_hack")]
    public string RsaHack(RsaEncryptMessage message)
    {
        return _rsaService.RsaHack(message.Text, message.N, message.E);
    }
    
    // Хэширование с использованием MD5
    [HttpPost]
    [Route("Md5_hash")]
    public string Md5Hash(HashMessage hashMessage)
    {
        return _md5Service.GetHash(hashMessage.Text);
    }

    // 4dc968ff0ee35c209572d4777b721587d36fa7b21bdc56b74a3dc0783e7b9518afbfa202a8284bf36e8e4b55b35f427593d849676da0d1d55d8360fb5f07fea2
    // 4dc968ff0ee35c209572d4777b721587d36fa7b21bdc56b74a3dc0783e7b9518afbfa200a8284bf36e8e4b55b35f427593d849676da0d1555d8360fb5f07fea2
    //                                                                        ^                                      ^         
    // 008ee33a9d58b51cfeb425b0959121c9
    // 008ee33a9d58b51cfeb425b0959121c9
    
    //d131dd02c5e6eec4693d9a0698aff95c2fcab58712467eab4004583eb8fb7f8955ad340609f4b30283e488832571415a085125e8f7cdc99fd91dbdf280373c5bd8823e3156348f5bae6dacd436c919c6dd53e2b487da03fd02396306d248cda0e99f33420f577ee8ce54b67080a80d1ec69821bcb6a8839396f9652b6ff72a70
    //d131dd02c5e6eec4693d9a0698aff95c2fcab50712467eab4004583eb8fb7f8955ad340609f4b30283e4888325f1415a085125e8f7cdc99fd91dbd7280373c5bd8823e3156348f5bae6dacd436c919c6dd53e23487da03fd02396306d248cda0e99f33420f577ee8ce54b67080280d1ec69821bcb6a8839396f965ab6ff72a70
    //                                      ^                                                   ^                           ^                                               ^                                                   ^                           ^             
    //79054025255fb1a26e4bc422aef54eb4
    //79054025255fb1a26e4bc422aef54eb4
    [HttpPost]
    [Route("Md5_file")]
    public string Md5Document(HashMessage hashMessage)
    {
        return _md5Service.GetDocumentHash(hashMessage.Text);
    }

    [HttpGet("find-collision")]
    public IActionResult FindCollision([FromQuery] int iterations = 1_000_000)
    {
        try
        {
            var result = _md5Service.FindCollision(iterations);

            if (result.HasValue)
            {
                return Ok(new
                {
                    Status = "Collision found",
                    FirstDocument = result.Value.FirstMessage,
                    SecondDocument = result.Value.SecondMessage,
                    CommonHash = result.Value.CommonHash,
                    Iterations = iterations
                });
            }

            return NotFound(new
            {
                Status = "Collision not found",
                Iterations = iterations,
                Message = "Try increasing the number of iterations"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Error",
                Message = ex.Message
            });
        }
    }
    
    [HttpPost]
    [Route("break-system")]
    public IActionResult BreakSystem([FromBody] MergeEncodeDto request)
    {
        var desEncrypted = _desService.Encrypt(request.Message, request.DesKey);
        
        var rsaEncrypted = _rsaService.RsaEncrypt(desEncrypted, request.n, request.e);
        
        var hackedDes = _rsaService.RsaHack(rsaEncrypted, request.n, request.e);
        
        var hackedDesKey = _desService.BruteForceDecrypt(hackedDes, request.KnownPlaintext);

        if (hackedDesKey == "Key not found")
        {
            return BadRequest("Не удалось подобрать DES ключ.");
        }

        var originalMessage = _desService.Decrypt(hackedDes, hackedDesKey);

        return Ok(new
        {
            Original = request.Message,
            DesEncrypted = desEncrypted,
            RsaEncrypted = rsaEncrypted,
            RsaHacked = hackedDes,
            DesKeyRecovered = hackedDesKey,
            MessageDecrypted = originalMessage
        });
    }
    
}