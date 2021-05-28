using System;
using Authenty;
using Authenty.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authenty.Test
{
    class Program
    {
        static void Main(string[] args)
        {

            Licensing GetLicensing = new Licensing(new Configuration()
            {
                RsaPubKey = "MIIDazCCAlOgAwIBAgIUY7t+peMSJ51Db1DK9XcOZOYM1CYwDQYJKoZIhvcNAQEFBQAwRTELMAkGA1UEBhMCVVMxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDAeFw0yMTAyMDUxNzQ1MTNaFw0zMTAyMDMxNzQ1MTNaMEUxCzAJBgNVBAYTAlVTMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBXaWRnaXRzIFB0eSBMdGQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDNgGEPvZxFynDV38tz5PMd/VSNZpq3iVCSXGRl3z9mVn2iJACz5oVxOezGZqszv+1ZPNNNovyMOurYDuWedhwgxOuaDnBfqMNahEWxDsbuachDEZee4TG+CaMOuGVKTVdx3nMN1yecwAxSypJxRwB1K4MH0Py+yxkj57P1HnrMFSMXO72P5bzhPV6NbXK/XBTVo5+h0q3puVZFNyrwNPeW75KPE0sApiI7MRZQHWlOrns1K87uo9dIyw/i4r+SxlHzwU+/zoOOfzqab4iX3XdrP31BakwH7Wkm9jDZlUPFwjrt5sHThFbZ7oNiBLPyT7vPX98CxmfkcxErJDHEtVf3AgMBAAGjUzBRMB0GA1UdDgQWBBSBAoNuXzKGQQ6s2BZ83ZBzTjxk2jAfBgNVHSMEGDAWgBSBAoNuXzKGQQ6s2BZ83ZBzTjxk2jAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBBQUAA4IBAQBz7ElQGn0lsbvSXwFzJbCMLfWF96lkSyX+BT0AfFTyHBle0AMJfJGDtVa7Y3CeosZqPxEpDFBIcoQ1nGlpeVa6Wk18aeKy/CUrZb40RDMm3X2/7FaLi7BVrxEKWHDXPcK2P03e4iEZ29rtptDuV3ng2H5kyPmTrC1It77c81xwkrQRQp7QzZWn96ChQRmD9hD3fcDp1Dh+qQqLimynD9wWSNJR3/pJNYEuoPRTpeW96ncakXk34Pv/u5vvwmP/DViXT3jW3K8+UaQyolPPJSNZA065hdzxH3qfaOhHARpABZl9BxLg9DLREPD09mxgcSuT6zUHyHDMd+b0LqcOvBUC",
                ApplicationId = 942469,
                ApplicationKey = "9eb56783c889a53c8aa7593dbb7d0bd5",
                ApplicationVersion = "1.0.0"
            });


            Console.ReadLine();

            GetLicensing.Connect();

            Console.WriteLine("1. Login");
            Console.WriteLine("2. register");

            switch (Console.ReadLine())
            {
                case "1":

                    Console.Clear();

                    Console.Write("user: ");
                    string user = Console.ReadLine();
                    Console.Write("password: ");
                    string pass = Console.ReadLine();

                    (bool SuccessLogged, string ServerMessage) = GetLicensing.Login(user, pass);

                    if (SuccessLogged)
                    {
                        Console.WriteLine(ServerMessage);

                        var (Success, Message) = GetLicensing.GetVariable("JmLr4V3OiP");

                        if (!Success)
                        {
                            Console.WriteLine(Message); // Specific Error Message
                            Console.ReadLine();
                            Environment.Exit(0);
                        }

                        Console.WriteLine(Message); // Successfully Logged Message

                    }
                    else
                    {
                        Console.WriteLine(ServerMessage);
                    }

                    break;

                case "2":

                    Console.Clear();
                    Console.Write("user: ");
                    string usern = Console.ReadLine();
                    Console.Write("password: ");
                    string passw = Console.ReadLine();
                    Console.Write("license: ");
                    string license = Console.ReadLine();

                    var registerUser = GetLicensing.Register(usern, passw, license, license);

                    Console.WriteLine(registerUser.ToString());


                    break;
            }

            Console.ReadLine();

        }
    }
}
