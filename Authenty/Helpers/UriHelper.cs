using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Authenty.Helpers
{
    public static class UriHelper
    {
        public static void Open(string url)
        {
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
                throw new UriFormatException(
                    "The format of the automatic updater of this application (updater download link) is not valid. If it continues, contact the developer.");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") {CreateNoWindow = true});
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                Process.Start("xdg-open", url);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                Process.Start("open", url);
            else
                throw new PlatformNotSupportedException();
        }
    }
}