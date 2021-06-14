using System;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Authenty.Manager
{
    public class UIDManager
    {
        public string Id => DiskID() + Cpuid() + WindowsID();

        private static string WindowsID()
        {
            var windowsInfo = "";
            var managClass = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_OperatingSystem");

            var managCollec = managClass.Get();

            var is64Bits = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"));

            foreach (var o in managCollec)
            {
                var managObj = (ManagementObject) o;
                windowsInfo = managObj.Properties["Caption"].Value + Environment.UserName +
                              (string) managObj.Properties["Version"].Value;
                break;
            }

            windowsInfo = windowsInfo.Replace("Windows", "").Replace("windows", "").Trim() +
                          (is64Bits ? " 64bit" : " 32bit");

            return BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(windowsInfo)))
                .Replace("-", "");
        }

        private string DiskID()
        {
            var diskLetter = string.Empty;
            //Find first drive
            foreach (var compDrive in DriveInfo.GetDrives())
            {
                if (!compDrive.IsReady) continue;

                diskLetter = compDrive.RootDirectory.ToString();
                break;
            }

            if (!string.IsNullOrEmpty(diskLetter) && diskLetter.EndsWith(":\\", StringComparison.Ordinal))
                diskLetter = diskLetter.Substring(0, diskLetter.Length - 2);

            var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + diskLetter + @":""");
            disk.Get();

            return disk["VolumeSerialNumber"].ToString();
        }

        [DllImport("user32", EntryPoint = "CallWindowProcW", CharSet = CharSet.Unicode, SetLastError = true,
            ExactSpelling = true)]
        private static extern IntPtr CallWindowProcW([In] byte[] bytes, IntPtr hWnd, int msg, [In, Out] byte[] wParam,
            IntPtr lParam);


        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualProtect([In] byte[] bytes, IntPtr size, int newProtect, out int oldProtect);

        private const int PageExecuteReadwrite = 0x40;

        private string Cpuid()
        {
            var sn = new byte[8];
            return !ExecuteCode(ref sn) ? "ND" : $"{BitConverter.ToUInt32(sn, 4):X8}{BitConverter.ToUInt32(sn, 0):X8}";
        }

        private bool ExecuteCode(ref byte[] result)
        {
            var code = IntPtr.Size == 8
                ? new byte[]
                {
                    0x53, 0x48, 0xc7, 0xc0, 0x01, 0x00, 0x00, 0x00, 0x0f, 0xa2, 0x41, 0x89, 0x00, 0x41, 0x89, 0x50,
                    0x04, 0x5b, 0xc3
                }
                : new byte[]
                {
                    0x55, 0x89, 0xe5, 0x57, 0x8b, 0x7d, 0x10, 0x6a, 0x01, 0x58, 0x53, 0x0f, 0xa2, 0x89, 0x07, 0x89,
                    0x57, 0x04, 0x5b, 0x5f, 0x89, 0xec, 0x5d, 0xc2, 0x10, 0x00
                };

            var ptr = new IntPtr(code.Length);

            if (!VirtualProtect(code, ptr, PageExecuteReadwrite, out _))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);
            return CallWindowProcW(code, IntPtr.Zero, 0, result, ptr) != IntPtr.Zero;
        }
    }
}