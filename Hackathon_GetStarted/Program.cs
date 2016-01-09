using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Hackathon_GetStarted
{
    class Program
    {
        static void Main(string[] args)
        {
            // Need to add better management for the args
            Console.WriteLine("################");
            Console.WriteLine("--> Enter your Login :");
            var lusername = Console.ReadLine();
            Console.WriteLine("--> Password :");
            var lpassword = Console.ReadLine();
            Console.WriteLine("--> VSTS Tenant :");
            var laccount = Console.ReadLine();
            Console.WriteLine("################");
            Console.WriteLine("### Initiate connexion :");
            Connexion(lusername, lpassword, laccount);


            Console.WriteLine("################");
            Console.WriteLine("END");
            Console.ReadKey();
        }

        public static async void Connexion(string username, string password, string account)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Hearder JSON
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    // Header Authentification
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", username, password))));
                    // Prepare the Request with the parameters
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=1.0", account);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        // Connexion Success
                        Console.WriteLine("## Connexion OK");
                        Console.WriteLine("## Projects already there : ");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Parse JSON and print only the Project Name
                        JsonTextReader reader = new JsonTextReader(new StringReader(responseBody));
                        // Trig only the parameter Name
                        bool print = false;
                        while (reader.Read())
                        {
                            if (reader.Value != null)
                            {
                                if (print)
                                {
                                    Console.WriteLine(reader.Value);
                                    print = false;
                                }
                                if ((reader.TokenType.ToString() == "PropertyName") && (reader.Value.ToString() == "name"))
                                {
                                    print = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
