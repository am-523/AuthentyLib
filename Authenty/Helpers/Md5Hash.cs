using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;

namespace Authenty.Helpers
{
    public class Md5Hash
    {
        public static string CurrentMd5File
        {
            get
            {
                using var md5Instance = MD5.Create();
                using var stream = File.OpenRead(Process.GetCurrentProcess().MainModule.FileName);
                var hashResult = md5Instance.ComputeHash(stream);
                return BitConverter.ToString(hashResult).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}