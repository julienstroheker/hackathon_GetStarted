using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hackathon_GetStarted.DomainServices
{
    class VSTSClient : IDisposable
    {
        private HttpClient _client;
        public string Tenant { get; set; }
        public string ProjectName { get; set; }

        public string _endpointTenantProjects { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=1.0", Tenant); } }
       

        private string _endpointGetProjects { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects/{1}?includeCapabilities=true&api-version=1.0", Tenant, ProjectName); } }
        public string _endpointCreateProject { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=2.0-preview", Tenant); } }
        public string _endpointCreateWIT { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/_apis/wit/workitems/$User%20Story?api-version=1.0", Tenant, ProjectName); } }
        private string _endpointBoardColumns { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/{1}%20Team/_apis/work/boards/Stories/columns?api-version=2.0-preview", Tenant, ProjectName); } }
        private string _endpointPushStyleConf { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/_apis/work/boards/Stories/cardrulesettings?api-version=2.0-preview.1", Tenant, ProjectName); } }

        public VSTSClient(string username,string password, string tenant, string projectName)
        {
            Tenant = tenant;
            ProjectName = projectName;
            _client = new HttpClient();
            //Hearder JSON
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // Header Authentification
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", username, password))));
        }

        public async Task ConfigureBoard()
        {
            try
            {
                // Request

                HttpResponseMessage response = _client.GetAsync(_endpointBoardColumns).Result;
                response.EnsureSuccessStatusCode();
                // Connexion Success
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON and print only the Project Name
                Console.WriteLine("- Got the Board");
                JObject resultJson = JObject.Parse(responseBody);
                // Trig only the parameter Name

                if ((string)resultJson["count"] == "4" && (string)resultJson["value"][0]["name"] == "New" && (string)resultJson["value"][1]["name"] == "Active" && (string)resultJson["value"][2]["name"] == "Resolved" && (string)resultJson["value"][3]["name"] == "Closed")
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

                    HttpResponseMessage responsePut = await _client.PutAsync(_endpointBoardColumns, request);
                    if (responsePut.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("- Colums modified");
                    }

                }
                else
                {
                    Console.WriteLine("Customisation already there");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task CreateProject(string newProjectName,string typeProject)
        {
            try
            {
                bool needCreated = false;
                // Request - Could be optimize with HEAD http request

                    HttpResponseMessage response = _client.GetAsync(_endpointGetProjects).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("- Projet doesn't exist");
                        needCreated = true;
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
                    // Prepare the Request with the parameters
                    // Request
                    var request = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage postResponse = await _client.PostAsync(_endpointCreateProject, request);
                            postResponse.EnsureSuccessStatusCode();
                            // Connexion Success
                            dynamic responseBody = JsonConvert.DeserializeObject( postResponse.Content.ReadAsStringAsync().Result);


                    var projectUrl = responseBody["url"];
                    Console.WriteLine(String.Format("Creation of the project Queued: {0}", projectUrl));

                   

                    bool created = false;
                    while (!created)
                    {
                        dynamic resultObj  = JsonConvert.DeserializeObject(_client.GetAsync((string)projectUrl).Result.Content.ReadAsStringAsync().Result);


                        created = resultObj["status"] == "succeeded";
                        Console.WriteLine(String.Format("Created: {0}", created));
                        Thread.Sleep(5000);
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



        public async void CreateUserStory(string title, string state, string tag, string witType, string assignedTo)
        {

            //Create JSON File for you new Task
            BoardNewItemProperty itemTitle = new BoardNewItemProperty("add", "/fields/System.Title", title);

            BoardNewItemProperty itemAreaPath = new BoardNewItemProperty("add", "/fields/System.AreaPath", ProjectName);

            BoardNewItemProperty itemTeamProject = new BoardNewItemProperty("add", "/fields/System.TeamProject", ProjectName);

            BoardNewItemProperty itemIterationPath = new BoardNewItemProperty("add", "/fields/System.IterationPath", ProjectName);

            BoardNewItemProperty itemWorkItemType = new BoardNewItemProperty("add", "/fields/System.WorkItemType", witType);

            BoardNewItemProperty itemState = new BoardNewItemProperty("add", "/fields/System.State", state);

            BoardNewItemProperty itemTags = new BoardNewItemProperty("add", "/fields/System.Tags", tag);

            IList<BoardNewItemProperty> theItem = new List<BoardNewItemProperty>();
            theItem.Add(itemTitle);
            theItem.Add(itemAreaPath);
            theItem.Add(itemTeamProject);
            theItem.Add(itemIterationPath);
            theItem.Add(itemWorkItemType);
            theItem.Add(itemState);
            theItem.Add(itemTags);

            if (assignedTo != null)
            {
                BoardNewItemProperty itemAssignedTo = new BoardNewItemProperty("add", "/fields/System.AssignedTo", assignedTo);
                theItem.Add(itemAssignedTo);
            }

            JArray theItemArray = JArray.FromObject(theItem);

            try
            {
                //Hearder JSON
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json-patch+json"));
                // Request
                var requestWIT = JsonConvert.SerializeObject(theItemArray);
                var request = new StringContent(requestWIT, System.Text.Encoding.UTF8, "application/json-patch+json");
                HttpResponseMessage resp = await _client.PatchAsync(_endpointCreateWIT, request);
                if (resp.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine("- Task {0} Created", title);
                    Thread.Sleep(500);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<List<string>> GetTenantProjects()
        {
            var projects = new List<string>();
            HttpResponseMessage response = _client.GetAsync(_endpointTenantProjects).Result;
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
                    projects.Add((string)responseJSON["value"][i]["name"]);
                }
            return projects;
            }

        private JObject BuildStyleConf()
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

        public async void PushStyleConf()
        {

            try
            {
                
                    string output = String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/_apis/work/boards/Stories/cardrulesettings?api-version=2.0-preview.1", laccount, newProjectName);
                    // Request
                    HttpResponseMessage response = _client.GetAsync(_endpointPushStyleConf).Result
                    
                    response.EnsureSuccessStatusCode();
                    // Connexion Success
                    Console.WriteLine("## Applying style on the board : ");
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Trig only the parameter Name
                    JObject configJSON = JObject.Parse(responseBody);
                    dynamic toPost = JsonConvert.DeserializeObject(responseBody);
                    toPost["rules"] = BuildStyleConf();
                    var content = (JsonConvert.SerializeObject(toPost));
                    var request = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                    Console.WriteLine(content);
                    await _client.PatchAsync(output, request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        public void Dispose()
        {
            _client.Dispose();
        }


    }
}
