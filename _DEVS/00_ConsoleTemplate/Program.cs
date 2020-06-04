using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;
using Helpers;
using System;

namespace graphconsoleapp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("graphconsoleapp.Program.Main --> START");

            var config = LoadAppSettings();
            if (config == null)
            {
                Console.WriteLine("Invalid appsettings.json file.");
                return;
            }

            var userName = "fabbio.b@brioches.onmicrosoft.com";//ReadUsername();
            var userPassword = ReadPassword();

            var client = GetAuthenticatedGraphClient(config, userName, userPassword);

            // request 1 - get user's files
            var request = client.Me.Drive.Root.Children.Request();

            var results = request.GetAsync().Result;
            foreach (var file in results)
            {
            Console.WriteLine(file.Id + ": " + file.Name);
            }

            Console.WriteLine("graphconsoleapp.Program.Main --> END");
        }

        #region Auth&Connect

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config, string userName, SecureString userPassword)
        {
            var authenticationProvider  = CreateAuthorizationProvider(config, userName, userPassword);
            var graphClient             = new GraphServiceClient(authenticationProvider);
            
            return graphClient;
        }

        private static HttpClient GetAuthenticatedHTTPClient(IConfigurationRoot config, string userName, SecureString userPassword)
        {
            var authenticationProvider = CreateAuthorizationProvider(config, userName, userPassword);
            var httpClient = new HttpClient(new AuthHandler(authenticationProvider, new HttpClientHandler()));

            return httpClient;
        }

        private static IAuthenticationProvider CreateAuthorizationProvider(IConfigurationRoot config, string userName, SecureString userPassword)
        {
            var clientId = config["applicationId"];
            var authority = $"https://login.microsoftonline.com/{config["tenantId"]}/v2.0";

            string[] scopes = config["scopes"].Split(",");

            var cca = PublicClientApplicationBuilder.Create(clientId)
                                                    .WithAuthority(authority)
                                                    .Build();

            return MsalAuthenticationProvider.GetInstance(cca, scopes, userName, userPassword);
        }

        #endregion Auth&Connect

        #region readAppSettings

        private static IConfigurationRoot LoadAppSettings()
        {
            try
            {
                var config = new ConfigurationBuilder()
                                  .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", false, true)
                                  .Build();

                if (string.IsNullOrEmpty(config["applicationId"]))
                {
                    Console.WriteLine("{0} is null", "applicationId"); return null;
                }
                if (string.IsNullOrEmpty(config["tenantId"]))
                {
                    Console.WriteLine("{0} is null", "tenantId"); return null;
                }
                if (string.IsNullOrEmpty(config["scopes"]))
                {
                    Console.WriteLine("{0} is null", "scopes"); return null;
                }

                return config;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
        }

        #endregion readAppSettings

        #region UsrPwd

        private static SecureString ReadPassword(bool isDefaultPwd = true)
        {
            SecureString password = new SecureString();

            if (isDefaultPwd)
            {
                char[] pwd = { 'B', '8', 'b', 'F', 'E', '4', 'q', 'J', 'F', '4', 'm', '7', 'u', 'c', 'N', 'Y' };
                foreach (var c in pwd)
                    password.AppendChar(c);
            }
            else
            {
                Console.WriteLine("Enter your password");
                while (true)
                {
                    ConsoleKeyInfo c = Console.ReadKey(true);
                    if (c.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                    password.AppendChar(c.KeyChar);
                    Console.Write("*");
                }
                Console.WriteLine();
            }

            return password;
        }

        private static string ReadUsername()
        {
            string username;
            Console.WriteLine("Enter your username");
            username = Console.ReadLine();
            return username;
        }

        #endregion UsPwd

    }
}
