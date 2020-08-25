using System;

public static class IdentifierGenerator
{
    public static string Generate() =>
      Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        .Replace("=", "")
        .Replace("+", "")
        .Replace("/", "");
}