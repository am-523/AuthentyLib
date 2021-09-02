## Authenty .NET Standard Library (https://biitez.dev/services/authenty)

[![GitHub Latest Release](https://img.shields.io/github/v/release/biitez/AuthentyLib.svg)](https://github.com/biitez/AuthentyLib/releases)
[![GitHub License](https://img.shields.io/github/license/biitez/AuthentyLib.svg)](https://github.com/biitez/AuthentyLib/blob/master/LICENSE)

### Features:
- Built using .NET Standard 2.0
- Broad compatibility with different versions of .NET
- Very easy to use, more secure and interactive library
- Compatibility with Windows Forms and WPF
- The methods are commented inside the library

### [Licensing](https://github.com/biitez/AuthentyLib/blob/master/Authenty/Licensing.cs)

- `void Connect()`
- `(bool Success, string Message) Login(string username, string password)`
- `(bool Success, string Message) LicenseLogin(string license)`
- `(bool Success, string Message) Register(string username, string password, string email, string license)`
- `(bool Success, string Message) ExtendSubscription(string username, string password, string license)`
- `(bool Success, string Message) GetVariable(string variableCode)`

### [Sample Console Application](https://github.com/biitez/AuthentyLib/blob/master/Authenty.Test/Program.cs)

Small example code:

```cs

Licensing GetLicensing = new Licensing(new Configuration {
    RsaPubKey = "Rsa Application Public Key",
    ApplicationId = 1234567,
    ApplicationKey = "Application Key",
    ApplicationVersion = "1.0.0" // Only required if your application uses an auto-updater
});

GetLicensing.Connect();

Console.WriteLine("Username: ");
string UserName = Console.ReadLine();

Console.WriteLine("Password: ");
string Password = Console.ReadLine();

var (SuccessLogged, ServerMessage) = GetLicensing.Login(UserName, Password);

if (SuccessLogged) {
   // User Logged In
} else {
   Console.WriteLine(ServerMessage); // Prints an error message
}

```


```
Compatibilities:
 _____________________________________________________________________
|                        |                                            |
|     Implementation     |                 Version                    |
|________________________|____________________________________________|
|                        |                                            |
| .NET Core & .NET 5.0   | 2.0 - 2.1 - 2.2 - 3.0 - 3.1 - 5.0          |
| .NET Framework         | 4.6.1 - 4.6.2 - 4.7 - 4.7.1 - 4.7.2 - 4.8  |
|  Mono                  | 5.4 - 6.4                                  |
|  Xamarin.iOS           | 10.14 - 12.16                              |
|  Xamarin.Android       | 8.0 - 10.0                                 |
|  Uni. Windows Platform | 10.0.16299 - TBD                           |
|  Unity                 | 2018.1                                     |
|________________________|____________________________________________|
```

##### This library is much more recommended than using the class in your project.

### Contributions, reports or suggestions
If you find a problem or have a suggestion inside this library, please let me know by [clicking here](https://github.com/biitez/AuthentyLib/issues), if you want to improve the code, make it cleaner or even more secure, create a [pull request](https://github.com/biitez/AuthentyLib/pulls). 

In case you will contribute in the code, please follow the same code base.

### Credits
This library and project has been built solely by me in my spare time, with a total of 620+ hours of work, please consider donations so I can continue to maintain the server, buy myself a coffee and keep writing/learning code for my GitHub.

- `Telegram: https://t.me/biitez`
- `Bitcoin Addy: bc1qzz4rghmt6zg0wl6shzaekd59af5znqhr3nxmms`
