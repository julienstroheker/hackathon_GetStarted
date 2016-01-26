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
            GetTemplateId();
            
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
                            test();
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
 
        public static async void AddStyle()
        {
            
        }
        public static JObject BuildConf()
        {
            List<BoardStyleFill> fills = new List<BoardStyleFill>();

            BoardStyleFillSettings fillSettingsPresentation = new BoardStyleFillSettings("#F5EEF8", "#000000");
            BoardStyleFillClauses fillClausesPresentation = new BoardStyleFillClauses("System.Tags", 1, "", "CONTAINS", "Presentation");
            List<BoardStyleFillClauses> fillClausesSPresentation = new List<BoardStyleFillClauses>();
            fillClausesSPresentation.Add(fillClausesPresentation);
            BoardStyleFill fillPresentation = new BoardStyleFill("Presentation", "True", "[System.Tags] contains 'Presentation'", fillClausesSPresentation, fillSettingsPresentation);
            fills.Add(fillPresentation);

            BoardStyleFillSettings fillSettingsLogistics = new BoardStyleFillSettings("#EAFFFF", "#000000");
            BoardStyleFillClauses fillClausesLogistics = new BoardStyleFillClauses("System.Tags", 1, "", "CONTAINS", "Logistics");
            List<BoardStyleFillClauses> fillClausesSLogistics = new List<BoardStyleFillClauses>();
            fillClausesSLogistics.Add(fillClausesLogistics);
            BoardStyleFill fillLogistics = new BoardStyleFill("Logistics", "True", "[System.Tags] contains 'Logistics'", fillClausesSLogistics, fillSettingsLogistics);
            fills.Add(fillLogistics);

            BoardStyleFillSettings fillSettingsLetsHack = new BoardStyleFillSettings("#FFFAE5", "#000000");
            BoardStyleFillClauses fillClausesLetsHack = new BoardStyleFillClauses("System.Tags", 1, "", "CONTAINS", "Let");
            List<BoardStyleFillClauses> fillClausesSLetsHack = new List<BoardStyleFillClauses>();
            fillClausesSLetsHack.Add(fillClausesLetsHack);
            BoardStyleFill fillLetsHack = new BoardStyleFill("Lets Hack", "True", "[System.Tags] contains 'Let'", fillClausesSLetsHack, fillSettingsLetsHack);
            fills.Add(fillLetsHack);

            BoardStyleFillSettings fillSettingsDemo = new BoardStyleFillSettings("#EFFFDC", "#000000");
            BoardStyleFillClauses fillClausesDemo = new BoardStyleFillClauses("System.Tags", 1, "", "CONTAINS", "Demonstrations");
            List<BoardStyleFillClauses> fillClausesSDemo = new List<BoardStyleFillClauses>();
            fillClausesSDemo.Add(fillClausesDemo);
            BoardStyleFill fillDemo = new BoardStyleFill("Demo", "True", "[System.Tags] contains 'Demonstrations'", fillClausesSDemo, fillSettingsDemo);
            fills.Add(fillDemo);


            List<BoardStyleTagStyle> tagsStyle = new List<BoardStyleTagStyle>();

            BoardStyleTagStyleSettings tagStyleSettingsPresentation = new BoardStyleTagStyleSettings("#00564B", "#FFFFFF");
            BoardStyleTagStyle tagStylePresentation = new BoardStyleTagStyle("Presentation", "True", tagStyleSettingsPresentation);
            tagsStyle.Add(tagStylePresentation);

            BoardStyleTagStyleSettings tagStyleSettingsLogistics = new BoardStyleTagStyleSettings("#2CBDD9", "#000000");
            BoardStyleTagStyle tagStyleLogistics = new BoardStyleTagStyle("Logistics", "True", tagStyleSettingsLogistics);
            tagsStyle.Add(tagStyleLogistics);

            BoardStyleTagStyleSettings tagStyleSettingsLetsHack = new BoardStyleTagStyleSettings("#FBFD52", "#000000");
            BoardStyleTagStyle tagStyleLetsHack = new BoardStyleTagStyle("Let's Hack", "True", tagStyleSettingsLetsHack);
            tagsStyle.Add(tagStyleLetsHack);

            BoardStyleTagStyleSettings tagStyleSettingsDemonstrations = new BoardStyleTagStyleSettings("#7ACE64", "#000000");
            BoardStyleTagStyle tagStyleDemonstrations = new BoardStyleTagStyle("Demonstrations", "True", tagStyleSettingsDemonstrations);
            tagsStyle.Add(tagStyleDemonstrations);


            BoardStyleRules confHackathon = new BoardStyleRules(fills, tagsStyle);

            

            JObject o = JObject.FromObject(confHackathon);

            return o;
        }

        public static async void test()
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
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/_apis/work/boards/Stories/cardrulesettings?api-version=2.0-preview.1", laccount, newProjectName);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        // Connexion Success
                        Console.WriteLine("## Getting configuration ");
                        string responseBody = await response.Content.ReadAsStringAsync();
                        // Trig only the parameter Name
                        JObject configJSON = JObject.Parse(responseBody);
                        dynamic toPost = JsonConvert.DeserializeObject(responseBody);
                        toPost["rules"] = BuildConf();
                        var content = (JsonConvert.SerializeObject(toPost));
                        var request = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                        Console.WriteLine(content);
                        await client.PatchAsync(output, request);
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
