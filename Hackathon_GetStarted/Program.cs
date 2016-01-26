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
            //Connexion().Wait();
            Console.WriteLine("### Get Template ID :");
            GetTemplateId();
            
            Console.WriteLine("### Create new demo projet :");


            using (var client = new VSTSClient(lusername, lpassword, laccount, newProjectName))
            {
                client.CreateProject(newProjectName, "Agile").Wait();
                Thread.Sleep(10000);
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
                Console.WriteLine("###################################################");


                //BuildConf();
                //test();
                Console.WriteLine("END");
                Console.WriteLine("###################################################");
                Console.WriteLine("Type any key to close this window...");
                Console.ReadKey();
            }
        }
        /*
        public static async Task Connexion()
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
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=1.0", laccount);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        // Connexion Success
                        Console.WriteLine("## Connexion OK");
                        Console.WriteLine("## Projects already there : ");
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
        */
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
        /*
        public static async void createHackathonProject(string typeProject)
        {
            try
            {
                bool needCreated = false;
                
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
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects/{1}?includeCapabilities=true&api-version=1.0", laccount, newProjectName);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Console.WriteLine("- Projet doesn't exist");
                            needCreated = true;
                        }
                    }
                }
                if (needCreated == true)
                {
                    dynamic postRequestObj = new JObject();
                    postRequestObj.name = newProjectName;
                    postRequestObj.description = "Hackathon Test App";
                    postRequestObj.capabilities = new JObject();
                    postRequestObj.capabilities.versioncontrol = new JObject();
                    postRequestObj.capabilities.versioncontrol.sourceControlType = "Git";
                    postRequestObj.capabilities.processTemplate = new JObject();
                    postRequestObj.capabilities.processTemplate.templateTypeId = "adcc42ab-9882-485e-a3ed-7678f01f66bc";

                    var jsonRequest = JsonConvert.SerializeObject(postRequestObj);

                    using (HttpClient client2 = new HttpClient())
                    {
                        //Hearder JSON
                        client2.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        // Header Authentification
                        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                            Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                    string.Format("{0}:{1}", lusername, lpassword))));
                        // Prepare the Request with the parameters
                        string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=2.0-preview", laccount);
                        var request = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                        // Request
                        using (HttpResponseMessage response = await client2.PostAsync(output, request))
                        {
                            response.EnsureSuccessStatusCode();
                            // Connexion Success
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("Creation of the project Queued");
                        }

                    }
                }
                else
                {
                    Console.WriteLine("!! Project already there, please remove it or change the name");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        */
        public static async void ConfigureBoard()
        {
            Thread.Sleep(5000);
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
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/{2}/_apis/work/boards/Stories/columns?api-version=2.0-preview", laccount, newProjectName, newTeamName);
                    // Request
                    using (HttpResponseMessage response = client.GetAsync(output).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        // Connexion Success
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Parse JSON and print only the Project Name
                        Console.WriteLine("- Got the Board");
                        JObject resultJson = JObject.Parse(responseBody);
                        // Trig only the parameter Name

                        if ((string)resultJson["count"]=="4" && (string)resultJson["value"][0]["name"]== "New"&& (string)resultJson["value"][1]["name"] == "Active" && (string)resultJson["value"][2]["name"] == "Resolved"&& (string)resultJson["value"][3]["name"] == "Closed")
                        {
                            int pFrom = responseBody.IndexOf("[");
                            int pTo = responseBody.LastIndexOf("]");

                            String updateColumsRequestString = responseBody.Substring(pFrom, pTo - pFrom + 1);
                            
                            JArray updateColumsRequest = JArray.Parse(updateColumsRequestString);
                            updateColumsRequest[1].Remove();
                            updateColumsRequest[0]["name"] = "TO DO";
                            updateColumsRequest[1]["name"] = "DOING";
                            updateColumsRequest[1]["stateMappings"]["User Story"] = "Active";
                            updateColumsRequest[2]["name"] = "DONE";

                            var updateColumsRequestPut = JsonConvert.SerializeObject(updateColumsRequest);

                            var request = new StringContent(updateColumsRequestPut, System.Text.Encoding.UTF8, "application/json");
                            Thread.Sleep(500);
                            Console.WriteLine("Pushing the new configuration for the board");
                            using (HttpResponseMessage responsePut = await client.PutAsync(output, request))
                            {
                                if (responsePut.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    Console.WriteLine("- Colums modified");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Customisation already there");
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
        /*
        public static async void createUserStory(string _title, string _state, string _tag, string _witType, string _assignedTo)
        {
           
            //Create JSON File for you new Task
            BoardNewItemProperty title = new BoardNewItemProperty("add", "/fields/System.Title", _title);

            BoardNewItemProperty areaPath = new BoardNewItemProperty("add", "/fields/System.AreaPath", newProjectName);

            BoardNewItemProperty teamProject = new BoardNewItemProperty("add", "/fields/System.TeamProject", newProjectName);

            BoardNewItemProperty iterationPath = new BoardNewItemProperty("add", "/fields/System.IterationPath", newProjectName);

            BoardNewItemProperty workItemType = new BoardNewItemProperty("add", "/fields/System.WorkItemType", _witType);

            BoardNewItemProperty state = new BoardNewItemProperty("add", "/fields/System.State", _state);

            BoardNewItemProperty tags = new BoardNewItemProperty("add", "/fields/System.Tags", _tag);

            

            IList<BoardNewItemProperty> theItem = new List<BoardNewItemProperty>();
            theItem.Add(title);
            theItem.Add(areaPath);
            theItem.Add(teamProject);
            theItem.Add(iterationPath);
            theItem.Add(workItemType);
            theItem.Add(state);
            theItem.Add(tags);

            if (_assignedTo != null)
            {
                BoardNewItemProperty assignedTo = new BoardNewItemProperty("add", "/fields/System.AssignedTo", _assignedTo);
                theItem.Add(assignedTo);
            }
            
            JArray theItemArray = JArray.FromObject(theItem);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //Hearder JSON
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json-patch+json"));
                    // Header Authentification
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                        Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                                string.Format("{0}:{1}", lusername, lpassword))));
                    // Prepare the Request with the parameters
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/_apis/wit/workitems/$User%20Story?api-version=1.0", laccount, newProjectName);
                    // Request
                    var requestWIT = JsonConvert.SerializeObject(theItemArray);
                    var request = new StringContent(requestWIT, System.Text.Encoding.UTF8, "application/json-patch+json");
                    HttpResponseMessage resp = await client.PatchAsync(output, request);
                    if (resp.StatusCode==System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("- Task {0} Created", _title);
                        Thread.Sleep(500);
                    }
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }

        }
        */
        public static void BuildConf()
        {
            List<BoardStyleFill> fills = new List<BoardStyleFill>();

            BoardStyleFillSettings fillSettingsPresentation = new BoardStyleFillSettings("#F5EEF8", "#000000");
            BoardStyleFillClauses fillClausesPresentation = new BoardStyleFillClauses("System.Tags", "1", "", "CONTAINS", "Presentation");
            List<BoardStyleFillClauses> fillClausesSPresentation = new List<BoardStyleFillClauses>();
            fillClausesSPresentation.Add(fillClausesPresentation);
            BoardStyleFill fillPresentation = new BoardStyleFill("Presentation", "True", "[System.Tags] contains 'Presentation'", fillClausesSPresentation, fillSettingsPresentation);
            fills.Add(fillPresentation);

            BoardStyleFillSettings fillSettingsLogistics = new BoardStyleFillSettings("#EAFFFF", "#000000");
            BoardStyleFillClauses fillClausesLogistics = new BoardStyleFillClauses("System.Tags", "1", "", "CONTAINS", "Logistics");
            List<BoardStyleFillClauses> fillClausesSLogistics = new List<BoardStyleFillClauses>();
            fillClausesSLogistics.Add(fillClausesLogistics);
            BoardStyleFill fillLogistics = new BoardStyleFill("Logistics", "True", "[System.Tags] contains 'Logistics'", fillClausesSLogistics, fillSettingsLogistics);
            fills.Add(fillLogistics);

            BoardStyleFillSettings fillSettingsLetsHack = new BoardStyleFillSettings("#FFFAE5", "#000000");
            BoardStyleFillClauses fillClausesLetsHack = new BoardStyleFillClauses("System.Tags", "1", "", "CONTAINS", "Let");
            List<BoardStyleFillClauses> fillClausesSLetsHack = new List<BoardStyleFillClauses>();
            fillClausesSLetsHack.Add(fillClausesLetsHack);
            BoardStyleFill fillLetsHack = new BoardStyleFill("Lets Hack", "True", "[System.Tags] contains 'Let'", fillClausesSLetsHack, fillSettingsLetsHack);
            fills.Add(fillLetsHack);

            BoardStyleFillSettings fillSettingsDemo = new BoardStyleFillSettings("#EFFFDC", "#000000");
            BoardStyleFillClauses fillClausesDemo = new BoardStyleFillClauses("System.Tags", "1", "", "CONTAINS", "Demonstrations");
            List<BoardStyleFillClauses> fillClausesSDemo = new List<BoardStyleFillClauses>();
            fillClausesSDemo.Add(fillClausesDemo);
            BoardStyleFill fillDemo = new BoardStyleFill("Demo", "True", "[System.Tags] contains 'Demonstrations'", fillClausesSDemo, fillSettingsDemo);
            fills.Add(fillDemo);


            List<BoardStyleTagStyle> tagsStyle = new List<BoardStyleTagStyle>();

            BoardStyleTagStyleSettings tagStyleSettingsPresentation = new BoardStyleTagStyleSettings("#00564B", "#FFFFFF");
            BoardStyleTagStyle tagStylePresentation = new BoardStyleTagStyle("Presentation", "True", tagStyleSettingsPresentation);
            tagsStyle.Add(tagStylePresentation);

            BoardStyleTagStyleSettings tagStyleSettingsLogistics = new BoardStyleTagStyleSettings("#2CBDD9", "#FFFFFF");
            BoardStyleTagStyle tagStyleLogistics = new BoardStyleTagStyle("Logistics", "True", tagStyleSettingsLogistics);
            tagsStyle.Add(tagStyleLogistics);

            BoardStyleTagStyleSettings tagStyleSettingsLetsHack = new BoardStyleTagStyleSettings("#FBFD52", "#FFFFFF");
            BoardStyleTagStyle tagStyleLetsHack = new BoardStyleTagStyle("Let's Hack", "True", tagStyleSettingsLetsHack);
            tagsStyle.Add(tagStyleLetsHack);

            BoardStyleTagStyleSettings tagStyleSettingsDemonstrations = new BoardStyleTagStyleSettings("#7ACE64", "#FFFFFF");
            BoardStyleTagStyle tagStyleDemonstrations = new BoardStyleTagStyle("Demonstrations", "True", tagStyleSettingsDemonstrations);
            tagsStyle.Add(tagStyleDemonstrations);


            BoardStyleRules confHackathon = new BoardStyleRules(fills, tagsStyle);

            JObject o = JObject.FromObject(confHackathon);


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
