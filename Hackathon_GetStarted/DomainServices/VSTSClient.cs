using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Hackathon_GetStarted.DomainServices
{
    class VSTSClient : IDisposable
    {
        private HttpClient _client;
        public string Tenant { get; set; }
        public string ProjectName { get; set; }

        private string _endpointGetProjects { get { return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects/{1}?includeCapabilities=true&api-version=1.0", Tenant, ProjectName); } }
        public string _endpointCreateProject { get{return String.Format("https://{0}.visualstudio.com/DefaultCollection/_apis/projects?api-version=2.0-preview", Tenant);}}
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

        public async void CreateHackathonProject(string newProjectName,string typeProject)
        {
            try
            {
                bool needCreated = false;
                // Request - Could be optimize with HEAD http request
                using (HttpResponseMessage response =  _client.GetAsync(_endpointGetProjects).Result)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Console.WriteLine("- Projet doesn't exist");
                        needCreated = true;
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
                    // Prepare the Request with the parameters
                    // Request
                    var request = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");
                    using (HttpResponseMessage response = await _client.PostAsync(_endpointCreateProject, request))
                        {
                            response.EnsureSuccessStatusCode();
                            // Connexion Success
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine("Creation of the project Queued");
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

        public void Dispose()
        {
            _client.Dispose();
        }


    }
}
