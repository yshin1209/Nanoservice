﻿#r "Newtonsoft.Json"

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Dynamic;
using System.Net.Http;
using System.Text;

public async static void Run(JObject eventGridEvent, TraceWriter log)
{
    log.Info(eventGridEvent.ToString(Formatting.Indented));
    var jsonEventGridEvent = eventGridEvent.ToString();
    JObject jobj = JObject.Parse(jsonEventGridEvent);
    double bloodSodiumValue = (double)jobj["data"]["value"];
    string actorId = (string)jobj["data"]["actorId"];
    //log.Info("bloodSodiumValue: " + bloodSodiumValue);
    //log.Info("(string)jobj[actorId]: " + actorId);
    string decision = "unknown";
    if (bloodSodiumValue < 135) { decision = "true"; }
    if (bloodSodiumValue >= 135) { decision = "false"; }
    HttpClient client = new HttpClient();
    string addVariableUri = "http://csmlab7.uconn.edu/addVariable";
    var firstRequest = new HttpRequestMessage(HttpMethod.Post, addVariableUri);
    dynamic firstRequestBody = new ExpandoObject();
    firstRequestBody.actorId = actorId;
    firstRequestBody.variable = "hyponatremia";
    firstRequestBody.value = "unknown";
    string firstJsonRequestBody = JsonConvert.SerializeObject(firstRequestBody);
    firstRequest.Content = new StringContent(firstJsonRequestBody, Encoding.UTF8, "application/json");
    var firstResponse = await client.SendAsync(firstRequest);

    string setValueUri = "http://csmlab7.uconn.edu/setValue";
    var secondRequest = new HttpRequestMessage(HttpMethod.Post, setValueUri);
    dynamic secondRequestBody = new ExpandoObject();
    secondRequestBody.actorId = actorId;
    secondRequestBody.variable = "hyponatremia";
    secondRequestBody.value = decision;
    string secondJsonRequestBody = JsonConvert.SerializeObject(secondRequestBody);
    secondRequest.Content = new StringContent(secondJsonRequestBody, Encoding.UTF8, "application/json");
    var secondResponse = await client.SendAsync(secondRequest);
}
