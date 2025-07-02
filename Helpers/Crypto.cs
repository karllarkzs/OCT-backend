namespace PharmaBack.Helpers;

using System.Security.Cryptography;

public static class BarcodeGenerator
{
    public static string GenerateInternalEan13()
    {
        var rnd = RandomNumberGenerator.GetInt32(0, 1_000_000_000);
        string body = $"480{rnd:D9}";

        int sum = 0;
        for (int i = 0; i < body.Length; i++)
        {
            int digit = body[i] - '0';
            sum += (i % 2 == 0) ? digit : digit * 3;
        }
        int check = (10 - (sum % 10)) % 10;
        return body + check;
    }
}
