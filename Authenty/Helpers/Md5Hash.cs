using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Authenty.Helpers
{
    public class Md5Hash
    {
        public static string CalculateFile(string location)
        {
            using (var md5Instance = MD5.Create())
            {
                using (var stream = File.OpenRead(location))
                {
                    var hashResult = md5Instance.ComputeHash(stream);
                    return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
