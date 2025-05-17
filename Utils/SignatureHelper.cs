using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using TransactionApi.Models;

namespace TransactionApi.Utils;

public static class SignatureHelper
{
    public static bool VerifySignature(TransactionRequest request, string partnerPasswordFromRequest)
    {
        string sigtimestamp = DateTime.Parse(
            request.timestamp,
            null,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal
        ).ToString("yyyyMMddHHmmss");
        string concatenated = sigtimestamp + request.partnerkey + request.partnerrefno + request.totalamount + partnerPasswordFromRequest;

        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(concatenated));
        string base64Hash = Convert.ToBase64String(hash);

        return base64Hash == request.sig.Trim();
    }
}