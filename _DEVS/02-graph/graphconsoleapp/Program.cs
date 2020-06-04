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
using System.IO;

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

            var userName        = "GradyA@brioches.onmicrosoft.com";//ReadUsername();
            var userPassword    = ReadPassword();
            
            var client          = GetAuthenticatedGraphClient(config, userName, userPassword);
            
            #region trending

            
            // request 1 - get trending files around a specific user (me)
            var request = client.Me.Insights.Used.Request();

            var results = request.GetAsync().Result;
            foreach (var resource in results)
            {
            Console.WriteLine("(" + resource.ResourceVisualization.Type + ") - " +resource.ResourceVisualization.Title);
            
            // - ottenuto con client.Me.Insights.Trending.Request();
            //Console.WriteLine("  Weight: " + resource.Weight);
            
            Console.WriteLine("  Id: " + resource.Id);
            Console.WriteLine("  ResourceId: " + resource.ResourceReference.Id);
            }

            #endregion trending


            #region DriveReadWrite
            
            //var request         = client.Me.Drive.Root.Children.Request();
            //var results         = request.GetAsync().Result;
            //
            //foreach (var file in results)
            //{ 
            //    var requestFile     = client.Me.Drive.Items[file.Id].Content.Request();
            //    var stream          = requestFile.GetAsync().Result;
            //    var driveItemPath   = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "driveItem_" + file.Name);
            //   
            //    if ( System.IO.File.Exists(driveItemPath) )
            //        System.IO.File.Delete(driveItemPath);
            //
            //    var driveItemFile   = System.IO.File.Create(driveItemPath);
            //    
            //    
            //    stream.Seek(0, SeekOrigin.Begin);
            //    stream.CopyTo(driveItemFile);
            //    
            //    Console.WriteLine(file.Id + ": " + file.Name + " --> Saved file to: " + driveItemPath);
            //
            //    driveItemFile.Close();
            //}
            //
            //Console.WriteLine("graphconsoleapp.Program.Main --> END");
            //
            ////FILE UPLOAD
            //// get reference to stream of file in OneDrive
            //var fileName        = "7.jpg";
            //var currentFolder   = System.IO.Directory.GetCurrentDirectory();
            //var filePath        = Path.Combine(currentFolder, fileName);
            //
            //// get a stream of the local file
            //FileStream fileStream = new FileStream(filePath, FileMode.Open);
            //
            //var uploadedFile = client.Me.Drive.Root
            //                            .ItemWithPath(fileName)
            //                            .Content
            //                            .Request()
            //                            .PutAsync<DriveItem>(fileStream)
            //                            .Result;
            //
            //
            //
            //results         = client.Me.Drive.Root.Children.Request().GetAsync().Result;
            //
            //foreach (var file in results)
            //{ 
            //    var requestFile     = client.Me.Drive.Items[file.Id].Content.Request();
            //    var stream          = requestFile.GetAsync().Result;
            //    var driveItemPath   = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "driveItem_" + file.Name);
            //    
            //    if ( System.IO.File.Exists(driveItemPath) )
            //        System.IO.File.Delete(driveItemPath);
            //        
            //    var driveItemFile   = System.IO.File.Create(driveItemPath);
            //    
            //    stream.Seek(0, SeekOrigin.Begin);
            //    stream.CopyTo(driveItemFile);
            //    
            //    Console.WriteLine(file.Id + ": " + file.Name + " --> Saved file to: " + driveItemPath);
            //
            //     driveItemFile.Close();
            //}
            //
            //
            //Console.WriteLine("START UPLOAD CHUNKED");
            //
            //var hugeFile        = "CELLI-SP_v2.1.pptx";
            //var hugeFilePath    = Path.Combine("C:\\temp\\", hugeFile);
            //
            //// load resource as a stream
            //using (Stream fStream = new FileStream(hugeFilePath, FileMode.Open))
            //{    
            //    var uploadSession = client.Me.Drive.Root
            //                        .ItemWithPath(hugeFile)
            //                        .CreateUploadSession()
            //                        .Request()
            //                        .PostAsync()
            //                        .Result;
            //
            //    // create upload task
            //    var maxChunkSize    = 320 * 1024;
            //    var largeUploadTask = new LargeFileUploadTask<DriveItem>(uploadSession, fStream, maxChunkSize);
            //
            //    // create upload progress reporter
            //    IProgress<long> uploadProgress = new Progress<long>(uploadBytes =>
            //    {
            //        Console.WriteLine($"Uploaded {uploadBytes} bytes of {fStream.Length} bytes");
            //    });
            //
            //    // upload file
            //    UploadResult<DriveItem> uploadResult = largeUploadTask.UploadAsync(uploadProgress).Result;
            //    if (uploadResult.UploadSucceeded)
            //    {
            //        Console.WriteLine("File uploaded to user's OneDrive root folder.");
            //    }
            //}
            #endregion DriveReadWrite

        }

        #region Auth&Connect

        private static GraphServiceClient GetAuthenticatedGraphClient(IConfigurationRoot config, string userName, SecureString userPassword)
        {
            var authenticationProvider = CreateAuthorizationProvider(config, userName, userPassword);
            var graphClient = new GraphServiceClient(authenticationProvider);

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
