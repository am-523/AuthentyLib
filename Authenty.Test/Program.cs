using System;
using Authenty.Models;

namespace Authenty.Test
{
    class Program
    {

        /// <summary>
        /// Here place the information of your application, 
        /// this information can be obtained in the Authenty panel.
        /// </summary>
        internal static Licensing GetLicensing = new Licensing(new Configuration
        {
            RsaPubKey = "MIIDazCCAlOgAwIBAgIUMVWPF8NG1nuTrhNP4Bz9C94rNKIwDQYJKoZIhvcNAQEFBQAwRTELMAkGA1UEBhMCVVMxEzARBgNVBAgMClNvbWUtU3RhdGUxITAfBgNVBAoMGEludGVybmV0IFdpZGdpdHMgUHR5IEx0ZDAeFw0yMTAyMDUxNzUwMTVaFw0zMTAyMDMxNzUwMTVaMEUxCzAJBgNVBAYTAlVTMRMwEQYDVQQIDApTb21lLVN0YXRlMSEwHwYDVQQKDBhJbnRlcm5ldCBXaWRnaXRzIFB0eSBMdGQwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQCg7ThQ/U06j2ulAmJQTx2aTdZu0CS07wYaw67ZQLLY4f6tl626r9RRV+jIK1u6GdcpkPqdphrh3rzOuNpNyx4nQZ+Q1s1DB2FDfFtI7UrXEGOA3xwOCf9JlF8rTo47h3AI2/ldKUki9BhNyX5ainy+RXP85R93hw8M9bTZkkiLD3e/xXJmuOzWSt6aC/3DjHfel7IEyj/LD7TEdkX1i1CUjozwGvHPifOThpB6cpJLGTzCjM7i8sFPMmD8R3rWwJ2s7b+74ET44rfnF59RaM/wtqagxI2WuD6GzsBVChtUKUcLzA/13MBOGMynOKCth8COQtVcDCqXw+FueWLo/LWDAgMBAAGjUzBRMB0GA1UdDgQWBBTKc+OuGA0WpPuDy8dfMC8liFnZhDAfBgNVHSMEGDAWgBTKc+OuGA0WpPuDy8dfMC8liFnZhDAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBBQUAA4IBAQANEw4nx6lGZp1Po88qxAfdiSbggtmWRpxv1Vm6pbmqVa1JNzgYwF5NiBZR1rm45AWCyXrN/PBCUYwLuZ6g6OYKRzSVj3O8JyacLHP7B5MQfWNm+Ez/b80QQd8FcKrvW+Fxo3ym7PR0y4M+NvyMFaZVAqLE3PyfNC0QUtABl7xL7MYQ4I0sSl5Qo+A93pI4xY4kG+vRvuZ/6vq1ayd1tEapnr8gkpaiWLg+Pi/tNKRWF40a0sppkM4fsw8KW1rQimP0EgWz1Sl1OfEBpEJG47HEq0YjqL0BGGtpmIBYP5gobbu8cOlyeDtSlulvVkAiq/IyzsL+w4TnNpGpGK10yxpZ",
            ApplicationId = 6519872,
            ApplicationKey = "9cfdf5db0fc343de91af41174b010b23",
            ApplicationVersion = "1.0.0"
        });

        static void Main(string[] args)
        {
            Console.Title = "Authenty - .NET Professional Licensing System";

            GetLicensing.Connect(); // Required.

            int UserInputInt;

            while (true)
            {
                Console.WriteLine("1. Login (Name, Password)");
                Console.WriteLine("2. Login (Only using the License)");
                Console.WriteLine("3. Register (Name, Email, License, Password)");
                Console.WriteLine("4. Extend Expiration Time Subscription");

                var UserInput = Console.ReadLine();

                if (!int.TryParse(UserInput, out UserInputInt) ||
                    UserInputInt < 1 || UserInputInt > 4)
                    Console.Clear();
                else
                    break;
            }

            Console.WriteLine();

            if (UserInputInt == 1) // Login (User name, Password)
            {
                Console.Write("User name: ");
                string UserName = Console.ReadLine();

                Console.Write("Password: ");
                string Password = Console.ReadLine();

                var (SuccessLogged, ServerMessage) = GetLicensing.Login(UserName, Password);

                // ServerMessage = Returns the error or success message in case the boolean is false / true.

                if (SuccessLogged)
                {
                    // Logged In
                    // Console.WriteLine(ServerMessage); // This write a successful login message.                    

                    YourMethodWhenTheUserIsLogged();
                }
                else
                {
                    // Invalid

                    Console.WriteLine();
                    Console.WriteLine(ServerMessage); // This writes the error message
                }

                // Or you can simply verify the data and put your own error message
                //if (GetLicensing.Login(UserName, Password).Success)
                //{
                //}
                //else
                //{
                //    // Invalid!
                //}

            }
            else if (UserInputInt == 2) // Login using a License Key
            {
                Console.Write("License: ");
                string LicenseKey = Console.ReadLine();

                var (SuccessLogged, ServerMessage) = GetLicensing.LicenseLogin(LicenseKey);

                // ServerMessage = Returns the error or success message in case the boolean is false / true.

                if (SuccessLogged)
                {
                    // Logged In!
                    YourMethodWhenTheUserIsLogged();
                }
                else
                {
                    // Invalid Login :(

                    Console.WriteLine();
                    Console.WriteLine(ServerMessage); // This print the error message
                }
            }
            else if (UserInputInt == 3) // Register (User name, Email, Password, License)
            {
                Console.Write("User name: ");
                string UserName = Console.ReadLine();

                Console.Write("Email Address: ");
                string Email = Console.ReadLine();

                Console.Write("Password: ");
                string Password = Console.ReadLine();

                Console.Write("License Key: ");
                string LicenseKey = Console.ReadLine();

                var (SuccessRegistered, ServerMessage) = GetLicensing.Register(UserName, Password, Email, LicenseKey);

                // ServerMessage = Returns the error or success message in case the boolean is false / true.

                if (SuccessRegistered)
                {
                    // Registered and Logged!
                    YourMethodWhenTheUserIsLogged();
                }
                else
                {
                    // Invalid :(
                    Console.WriteLine(ServerMessage); // This print the error message
                }
            }
            else if (UserInputInt == 4) // Extend Expiration Time Subscription
            {
                /*
                 * The user must enter his user name / password together with 
                 * a license key with more time, this method will make the 
                 * expiration time of his account lengthen depending on the 
                 * time of the license key that he placed.
                 * 
                 * PS: If you use the license login and not the user name + password, 
                 * you must put the user to enter their license key and then, in the 
                 * method send that variable in the user name and password parameters.
                 */

                Console.Write("User name: ");
                string UserName = Console.ReadLine();

                Console.Write("Password: ");
                string Password = Console.ReadLine();

                Console.Write("License with more expiration date: ");
                string License = Console.ReadLine();

                var (SuccessExtendedTime, ServerMessage) = GetLicensing.ExtendSubscription(UserName, Password, License);

                // ServerMessage = Returns the error or success message in case the boolean is false / true.

                if (SuccessExtendedTime)
                {
                    // Extended subscription, at this point, the user must re-login using their account.
                    Environment.Exit(0);
                }
                else
                {
                    // Invalid
                    Console.WriteLine(ServerMessage);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }

            Console.ReadLine();
        }


        static void YourMethodWhenTheUserIsLogged()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("1. View User Info");
                Console.WriteLine("2. Get Secure Remote Variables (Very Recommended)");

                if (!int.TryParse(Console.ReadLine(), out int UserLoggedInput) ||
                    UserLoggedInput < 1 || UserLoggedInput > 2)
                    continue;

                Console.WriteLine();

                if (UserLoggedInput == 1)
                {
                    Console.WriteLine($"User Name: {GetLicensing.UserInformation.Username}");
                    Console.WriteLine($"Email Address: {GetLicensing.UserInformation.Email}");
                    Console.WriteLine($"Expire Date: {GetLicensing.UserInformation.ExpireDate}");
                    Console.WriteLine($"HWID: {GetLicensing.UserInformation.Hwid}");
                    Console.WriteLine($"Level: {GetLicensing.UserInformation.Level}");

                    Console.WriteLine("Enter to return");

                    Console.ReadLine();

                    continue;
                }
                else
                {
                    Console.Write("Secret Variable Code: ");

                    string VariableCode = Console.ReadLine().Trim();

                    var (SuccessVariable, VariableValue) = GetLicensing.GetVariable(VariableCode);

                    /* 
                     * If it returns a false boolean, it means 
                     * that you are trying to get the remote 
                     * variable before the user is logged in, 
                     * for security you can only get the remote 
                     * variables from the panel when the user 
                     * logs in correctly.
                     * 
                     * If it is some other error, at this point there will be an exception with reason.
                     */

                    if (SuccessVariable)
                    {
                        Console.WriteLine(Environment.NewLine + $"Secret Variable Value: {VariableValue}");

                        Console.WriteLine(Environment.NewLine + "Press enter to return.");

                        Console.ReadLine();

                        continue;
                    }
                }
            }
        }
    }
}
