using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using Hackathon_GetStarted.DomainServices;

namespace Hackathon_GetStarted
{
    class Program
    {
        static string lusername;
        static string lpassword;
        static string laccount;
        static Dictionary<string,string> templateProjectID = new Dictionary<string, string>();
        static string newProjectName = "";
        static string newTeamName = "";

        static void Main(string[] args)
        {
            
            // Need to add better management for the args
            Console.WriteLine("###################################################");
            Console.WriteLine("------> Enter your Login (Basic Authentification must be activated) :");
            lusername = Console.ReadLine();
            Console.WriteLine("------> Password :");
            lpassword = Console.ReadLine();
            Console.WriteLine("------> VSTS Tenant (https://XXXXXXXXXX.visualstudio.com) :");
            laccount = Console.ReadLine();
            Console.WriteLine("###################################################");
            Console.WriteLine("------> Project name :");
            newProjectName = Console.ReadLine();
            newTeamName = newProjectName + "%20Team";
            Console.WriteLine("###################################################");
            Console.WriteLine("### Initiating connexion :");
            Console.WriteLine("### Get Template ID :");
            
            Console.WriteLine("### Create new demo projet :");


            using (var client = new VSTSClient(lusername, lpassword, laccount, newProjectName))
            {
                var projects = client.GetTenantProjects().Result;
                foreach (var project in projects)
                {
                    Console.WriteLine(project);
                }               
                client.CreateProject(newProjectName, "Agile")
                        .ContinueWith(_ => {
                            client.ConfigureBoard().Wait();

                            Thread.Sleep(5000);
                            Console.WriteLine("------> Do you want to assign the tasks to someone ? Y/N");
                            string assignatedYorN = Console.ReadLine();
                            string assignedTO = null;
                            if (assignatedYorN.ToUpper() == "Y")
                            {
                                Console.WriteLine("### Assign Tasks to :\n Use the correct type like this \"First Name <xxxx@outlook.com>\"\n Example : Julien Stroheker <julien.stroheker@outlook.com>");
                                assignedTO = Console.ReadLine();
                            }
                            client.CreateUserStory("Welcome Guests", "Closed", "Logistics", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Demonstrate Kanban", "Active", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Explain how the Hackathon will work ?", "Active", "Presentation", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("DevOps Overview", "Active", "Presentation", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Demonstrate APM + Automated Recovery Parts Unlimited App", "New", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);

                            client.CreateUserStory("Demonstrate Cloud Based Load Testing + Autoscale policies in IaC", "New", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Demonstrate Infrastructure as Code with Azure Resource Manager Deployment Templates", "New", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Demonstrate Continuous Deployment and Release Management with Visual Studio Team Services", "New", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Demonstrate Continuous Integration with Visual Studio Team Services", "New", "Demonstrations", "User Story", assignedTO);
                            Thread.Sleep(1000);

                            client.CreateUserStory("Let’s form our teams!", "New", "Let's Hack", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            client.CreateUserStory("Activate your Tools", "New", "Let's Hack", "User Story", assignedTO);
                            Thread.Sleep(1000);
                            Thread.Sleep(5000);
                            client.PushStyleConf();
                            Console.WriteLine("###################################################");


                            //BuildConf();
                            //test();
                            Console.WriteLine("END");
                            Console.WriteLine("###################################################");
                            Console.WriteLine("Type any key to close this window...");
                            Console.ReadKey();

                        }).Wait();

            }
        }
        public static async void GetTemplateId()
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
                                string.Format("{0}:{1}", lusername, lpassword))));
                    // Prepare the Request with the parameters
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/process/processes?api-version=1.0", laccount);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        // Connexion Success
                        Console.WriteLine("## List Template");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Parse JSON and print only the Project Name
                        JObject responseJSON = JObject.Parse(responseBody);
                        int nbProjects = (int)responseJSON["count"];
                        for (int i = 0; i < nbProjects; i++)
                        {
                            Console.WriteLine("- " + (string)responseJSON["value"][i]["name"]);
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
