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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace NanoserviceAPI.Controllers
{
    //[Consumes("application/json")]
    //[Produces("application/json")]
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
        /// Try this service:
        ///
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Value | Model" textbox (white). A sample request body is shown above.
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
        ///        "variable": "bloodSodium",              
        ///        "value": "unknown"                     
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// - "value": string, int, float, bool
        /// 
        /// Try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Value | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). Returns true if the value was successfully added and false if an actor state with the same name already exists.
        /// </remarks>
        [HttpPost]
        [Route("tryAddVariable")]
        public async Task<dynamic> TryAddVariable([FromBody] JObject requestBody)
        {
            var actorId = (string) requestBody.SelectToken("actorId");
            var variable = (string) requestBody.SelectToken("variable");
            JTokenType valueType = requestBody.SelectToken("value").Type;
            dynamic value = "arbitrary value"; // initialize dynamic type value
            switch (valueType)
            {   
                case JTokenType.Float:
                    value = (float)requestBody.SelectToken("value");
                    break;
                case JTokenType.Boolean:
                    value = (bool)requestBody.SelectToken("value");
                    break;
                case JTokenType.Integer:
                    value = (int)requestBody.SelectToken("value");
                    break;
                case JTokenType.String:
                    value = (string)requestBody.SelectToken("value");
                    break;
            }

            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            var response = await actor.TryAddVariableAsync(variable, value);
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
        ///        "variable": "bloodSodium"                  
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// 
        /// Try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Value  | Model" textbox (white). A sample request body is shown above.
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
            dynamic returnObj = new ExpandoObject();
            returnObj.value = response;
            return returnObj;
        }

        /// <summary>
        /// Sets the value of a variable.
        /// </summary>
        /// <remarks>
        /// Sample request body (requestBody):
        ///
        ///     {
        ///        "actorId": "patient032904475",    
        ///        "variable": "bloodSodium",              
        ///        "value": 109,
        ///        "publish": true
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// - "value": string, int, float, bool
        /// - "publish": bool
        /// 
        /// Try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Value  | Model" textbox (white). A sample request body is shown above.
        /// 3. Click [Execute] button (blue).
        /// 4. Check "Response body" (ignore "Code" for now). If an actor variable with specified name does not exist, it is added. If you see "Value set", your request is processed successfully. Otherwise, you will get an error messsage. 
        /// </remarks>

        [HttpPost]
        [Route("setValue")]
        public async Task<dynamic> SetValue ([FromBody] dynamic requestBody)
        {
            var actorId = (string)requestBody.SelectToken("actorId");
            var variable = (string) requestBody.SelectToken("variable");
            JTokenType valueType = requestBody.SelectToken("value").Type;
            dynamic value = "any value"; // initialize dynamic type value
            switch (valueType)
            {   
                case JTokenType.Float:
                    value = (float)requestBody.SelectToken("value");
                    break;
                case JTokenType.Boolean:
                    value = (bool)requestBody.SelectToken("value");
                    break;
                case JTokenType.Integer:
                    value = (int)requestBody.SelectToken("value");
                    break;
                case JTokenType.String:
                    value = (string)requestBody.SelectToken("value");
                    break;
            }

            var actor = ActorProxy.Create<IActors>(new ActorId(actorId), new Uri("fabric:/Nanoservice/ActorsActorService"));
            string response = await actor.SetValueAsync(variable, value);
            var publish = (bool)requestBody.SelectToken("publish");
            if (publish == true)
            {
                await PublishToAzureEventGridAsync(requestBody); // publish to Azure Event Grid
            }
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
        ///        "variable": "bloodSodium"                  
        ///     }
        /// 
        /// - "actionId": string
        /// - "variable": string
        /// 
        /// Try this service:
        /// 
        /// 1. Click [Try it out] button (white).
        /// 2. Type your request body into "Example Value  | Model" textbox (white). A sample request body is shown above.
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

        public async Task PublishToAzureEventGridAsync(JObject data)
        {
            string AzureEventGridTopicEndPoint = "https://topic.eastus-1.eventgrid.azure.net/api/events?api-version=2018-01-01";
            string AzureEventGridTopicAccessKey = "bHLip04YkH3Ysh0WvISAEUINVk3BWcPGTqGB6t/0iQw=";
            // [Warning] Be careful not to add "/" at the end of publisherBaseUri
            //string publisherBaseUri = "http://nanoservice3.eastus.cloudapp.azure.com";
            string publisherBaseUri = "http://csmlab7.uconn.edu";
            string uri = AzureEventGridTopicEndPoint;
            string topicSubject = (string)data.SelectToken("variable");

            data.Add("publisherBaseUri", publisherBaseUri);

            // Event data schema (Azure Event Grid)
            // https://docs.microsoft.com/en-us/azure/event-grid/post-to-custom-topic#event-data

            dynamic requestBody = new ExpandoObject();
            requestBody.id = "notSet";
            requestBody.eventType = "notSet";
            requestBody.subject = topicSubject; // e.g., bloodSodium
            requestBody.eventTime = DateTime.Now;
            requestBody.data = data;
            requestBody.dataVersion = "v1";

            List<dynamic> requestBodyArray = new List<dynamic>();
            requestBodyArray.Add(requestBody);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("aeg-sas-key", AzureEventGridTopicAccessKey);
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                string jsonRequestBody = JsonConvert.SerializeObject(requestBodyArray);
                request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
                await client.SendAsync(request);
            }
        }
    }
}
 