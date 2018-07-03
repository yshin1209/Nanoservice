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
using System.Collections.Generic;

namespace NanoserviceAPI.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    public class ActorsController : Controller
    {
        /// <summary>
        /// Creates an actor.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475" 
        ///     }
        ///     
        /// - "actionId": string
        /// 
        /// To try this service:
        ///
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Vaule | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If you see "Actor created", your request is processed successfully. Otherwise, you will get an error messsage.
        /// 
        /// For more information about Service Fabric Actors, please see:
        /// https://docs.microsoft.com/en-us/azure/service-fabric/service-fabric-reliable-actors-introduction
        /// </remarks>
        [HttpPost]
        [Route("createActor")]
        public string CreateActor([FromBody] JObject requestBody)
        {
            try
            {
                var actorId = (string)requestBody.SelectToken("actorId");
                var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
                return ("Actor created");
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        /// <summary>
        /// Adds a variable to an actor.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475",    
        ///        "variable": "sodium",              
        ///        "value": 142                      
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// - "value": string, int, float, bool
        /// 
        /// To try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Vaule | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If you see "Variable added", your request is processed successfully. Otherwise, you will get an error messsage.
        /// </remarks>
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

        /// <summary>
        /// Gets the value of a variable.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475",    
        ///        "variable": "sodium"                  
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// 
        /// To try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Vaule | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If you see the value retrieved, your request is processed successfully. Otherwise, you will get an error messsage.
        /// </remarks>
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

        /// <summary>
        /// Sets the value of a variable.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475",    
        ///        "variable": "sodium",              
        ///        "value": 109                      
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// - "value": string, int, float, bool
        /// 
        /// To try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Vaule | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If you see "Value set", your request is processed successfully. Otherwise, you will get an error messsage.
        /// </remarks>

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
        /// <summary>
        /// Removes a variable from an actor.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475",    
        ///        "variable": "sodium"                  
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// 
        /// To try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Vaule | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If you see "Variable removed", your request is processed successfully. Otherwise, you will get an error messsage.
        /// </remarks>
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

        public async Task<string> PublishAsync(JObject data)
        {
            string AzureEventGridTopicEndPoint = "https://topic.eastus-1.eventgrid.azure.net/api/events?api-version=2018-01-01";
            string AzureEventGridTopicAccessKey = "9UGRYFbXX3Pqr8yTp2vvhgvNBr8HO0HSWza/PMdxu/0=";
            string uri = AzureEventGridTopicEndPoint;
            string topicSubject = (string)data.SelectToken("variable"); 
            string jsonData = JsonConvert.SerializeObject(data);

            // Event data schema (Azure Event Grid)
            // https://docs.microsoft.com/en-us/azure/event-grid/post-to-custom-topic#event-data

            dynamic requestBody = new ExpandoObject();
            requestBody.id = "notSet";
            requestBody.eventType = "notSet";
            requestBody.subject = topicSubject; // e.g., BloodSodium
            requestBody.eventTime = DateTime.Now;
            requestBody.data = jsonData;
            requestBody.dataVersion = "v1";

            List<dynamic> requestBodyArray = new List<dynamic>();
            requestBodyArray.Add(requestBody);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("aeg-sas-key", AzureEventGridTopicAccessKey);
            var request = new HttpRequestMessage(HttpMethod.Post, uri);
            string jsonRequestBody = JsonConvert.SerializeObject(requestBodyArray);
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            string stringResponseContent = await response.Content.ReadAsStringAsync();
            return stringResponseContent;
        }
    }
}
 