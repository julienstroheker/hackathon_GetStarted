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

        private string _endpointGetProjects { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects/{1}?includeCapabilities=true&api-version=1.0", Tenant, ProjectName); } }
        private string _endpointCreateProject { get{return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=2.0-preview", Tenant);}}
        private string _endpointBoardColumns { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/{1}/{1}%20Team/_apis/work/boards/Stories/columns?api-version=2.0-preview", Tenant, ProjectName); } }

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

        public async Task CreateProject(string newProjectName,string typeProject)
        {
            try
            {
                bool needCreated = false;
                // Request - Could be optimize with HEAD http request
                HttpResponseMessage getResponse =  _client.GetAsync(_endpointGetProjects).Result;
                if (getResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
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
                            string responseBody = await postResponse.Content.ReadAsStringAsync();
                            Console.WriteLine("Creation of the project Queued");
                                         
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

        public void Dispose()
        {
            _client.Dispose();
        }


    }
}
