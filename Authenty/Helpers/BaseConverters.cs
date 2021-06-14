using System;

namespace Authenty.Helpers
{
    public static class BaseConverters
    {
        internal static string ToUrlSafeBase64(byte[] input)
        {
            return Convert.ToBase64String(input).Replace("+", "-").Replace("/", "_");
        }

        internal static byte[] FromUrlSafeBase64(string input)
        {
            return Convert.FromBase64String(input.Replace("-", "+").Replace("_", "/"));
        }
    }
}