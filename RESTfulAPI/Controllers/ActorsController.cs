using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors.Client;
using Actors.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using RESTfulAPI;

namespace NanoserviceAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ActorsController : Controller
    {
        [HttpPost]
        [Route("createActor")]
        public Task<string> CreateActor([FromBody] JObject requestBody)
        {
            var actorId = (string)requestBody.SelectToken("actorId");
            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            return Task.FromResult<string>("Actor created");
        }

        [HttpPost]
        [Route("addVariable")]
        public async Task<string> AddVariable([FromBody] JObject requestBody)
        {
            var actorId = (string) requestBody.SelectToken("actorId");
            var variable = (string) requestBody.SelectToken("variable");
            JTokenType valueType = requestBody.SelectToken("value").Type;
            dynamic value = "any value"; // initialize dynamic type value
            switch (valueType)
            {
                case JTokenType.Boolean:
                    value = (bool)requestBody.SelectToken("value");
                    break;
                case JTokenType.Integer:
                    value = (int)requestBody.SelectToken("value");
                    break;
                case JTokenType.String:
                    value = (string)requestBody.SelectToken("value");
                    break;
                case JTokenType.Float:
                    value = (float)requestBody.SelectToken("value");
                    break;
            }

            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            var response = await actor.AddVariableAsync(variable, value);
            await PublishAsync(requestBody);
            return response;
        }

        [HttpPost]
        [Route("setValue")]
        public async Task<string> SetValue ([FromBody] JObject requestBody)
        {
            var actorId = (string)requestBody.SelectToken("actorId");
            var variable = (string) requestBody.SelectToken("variable");
            JTokenType valueType = requestBody.SelectToken("value").Type;
            dynamic value = "any value"; // initialize dynamic type value
            switch (valueType)
            {
                case JTokenType.Boolean:
                    value = (bool)requestBody.SelectToken("value");
                    break;
                case JTokenType.Integer:
                    value = (int)requestBody.SelectToken("value");
                    break;
                case JTokenType.String:
                    value = (string)requestBody.SelectToken("value");
                    break;
                case JTokenType.Float:
                    value = (float)requestBody.SelectToken("value");
                    break;
            }

            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            string response = await actor.SetValueAsync(variable, value);
            await PublishAsync(requestBody);
            return response;
        }

        [HttpPost]
        [Route("getValue")]
        public async Task<dynamic> GetValue([FromBody] JObject requestBody)
        {
            var actorId = (string)requestBody.SelectToken("actorId");
            var variable = (string)requestBody.SelectToken("variable");
            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            dynamic response = await actor.GetValueAsync(variable);
            return response;
        }

        [HttpPost]
        [Route("removeVariable")]
        public async Task<string> RemoveVariable([FromBody] JObject requestBody)
        {
            var actorId = (string)requestBody.SelectToken("actorId");
            var variable = (string)requestBody.SelectToken("variable");
            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            string response = await actor.RemoveVariableAsync(variable);
            return response;
        }


        public async Task<string> PublishAsync(dynamic data)
        {
            string AzureEventGridTopicEndPoint = "https://topic.eastus-1.eventgrid.azure.net/api/events?api-version=2018-01-01";
            string AzureEventGridTopicAccessKey = "9UGRYFbXX3Pqr8yTp2vvhgvNBr8HO0HSWza/PMdxu/0=";
            string uri = AzureEventGridTopicEndPoint;
            string topicSubject = data.stateVariableName;
            string jsonData = JsonConvert.SerializeObject(data);

            // Event data schema (Azure Event Grid)
            // https://docs.microsoft.com/en-us/azure/event-grid/post-to-custom-topic#event-data

            dynamic requestBody = new ExpandoObject();
            requestBody.id = "";
            requestBody.eventType = "";
            requestBody.subject = topicSubject; // e.g., BloodSodium
            requestBody.eventTime = DateTime.Now;
            requestBody.data = jsonData;
            requestBody.dataVersion = "";

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("aeg-sas-key", AzureEventGridTopicAccessKey);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string stringResponseContent = await response.Content.ReadAsStringAsync();
            return stringResponseContent;
        }
    }
}
 