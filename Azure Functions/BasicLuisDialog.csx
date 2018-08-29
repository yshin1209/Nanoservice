// BasicLuisDialog.csx
#load "HemoglobinForm.csx"
#load "MCVForm.csx"
#load "SerumFerritinForm.csx"

using System;
using System.Configuration;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Dynamic;
using System.Net.Http;
using System.Text;

using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
[LuisModel("efecc7a6-ffdb-45f2-90de-908c8edffa3f", "3f3d532a6766431d9b911c8577e998df")]
public class BasicLuisDialog : LuisDialog<object>
{
    string patientID = "";
    string getValueUri = "http://csmlab7.uconn.edu/getValue";
    string setValueUri = "http://csmlab7.uconn.edu/setValue";
    string nextBotStatement ="";
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
        ConfigurationManager.AppSettings["LuisAppId"], 
        ConfigurationManager.AppSettings["LuisAPIKey"], 
        domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
    {
    }

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync($"I did not understand what you said: {result.Query}");
        context.Wait(MessageReceived);
    }

    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "Greeting" with the name of your newly created intent in the following handler
    [LuisIntent("Greeting")]
    public async Task GreetingIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync("Which blood test do you want to talk about? (e.g., hemoglobin)");
    }

    [LuisIntent("TalkAboutHemoglobin")]
    public async Task TalkAboutHemoglobinIntent(IDialogContext context, LuisResult result)
    {
        await context.PostAsync("Hemoglobin (Hb or Hgb) is a protein in red blood cells that carries oxygen. For more information, please visit https://medlineplus.gov/labtests/hemoglobintest.html.");
        System.Threading.Thread.Sleep(4000);
        context.Call(HemoglobinForm.BuildFormDialog(FormOptions.PromptInStart), HemoglobinFormComplete);  
        //context.Call(MCVForm.BuildFormDialog(FormOptions.PromptInStart), MCVFormComplete);
    }

    private async Task HemoglobinFormComplete(IDialogContext context, IAwaitable<HemoglobinForm> result)
    {
        try
        {
            var form = await result;
            patientID = form.PatientID;

            if (form != null)
            {
                //Set varialbe values
                var genderValue = ""; 
                if ((int)form.Gender == 1) {genderValue = "female";}
                if ((int)form.Gender == 2) {genderValue = "male";}
                await SetValue(patientID, setValueUri, "gender", genderValue, "no");
                await SetValue(patientID, setValueUri, "age", form.Age, "no");
                await SetValue(patientID, setValueUri, "hemoglobin", form.HemoglobinValue, "yes");
                System.Threading.Thread.Sleep(2000);
                nextBotStatement = GetValue(patientID, getValueUri, "nextBotStatement");
                await context.PostAsync(nextBotStatement);
            }
            else
            {
                await context.PostAsync("Form returned empty response! Type anything to restart it.");
            }
        }
        catch (OperationCanceledException)
        {
            await context.PostAsync("You canceled the form! Type anything to restart it.");
        }
        string lowHemoglobin = GetValue(patientID, getValueUri, "lowHemoglobin");
        if (lowHemoglobin == "yes")
        {
            System.Threading.Thread.Sleep(2000);
            await context.PostAsync("To find out the cause of your low hemoglobin, I need your Mean Corpuscular Volume (MCV). MCV shows the size of red blood cells. For example, if MCV is low red blood cells are small. If you want to know more about MCV, please visit https://medlineplus.gov/labtests/mcvmeancorpuscularvolume.html.");
            System.Threading.Thread.Sleep(5000);
            context.Call(MCVForm.BuildFormDialog(FormOptions.PromptInStart), MCVFormComplete);
        } else if (lowHemoglobin == "no")
        {
            System.Threading.Thread.Sleep(3000);
            await context.PostAsync("This is the end of the simulated case. Please type 'restart' to start over.");
        }
    }

    private async Task MCVFormComplete(IDialogContext context, IAwaitable<MCVForm> result)
    {
        try
        {
            var form = await result;
            double MCV = form.MCV;
            if (form != null)
            {
                await SetValue(patientID, setValueUri, "MCV", MCV, "yes");
                System.Threading.Thread.Sleep(2000);
                nextBotStatement = GetValue(patientID, getValueUri, "nextBotStatement");
                await context.PostAsync(nextBotStatement);
            }
            else
            {
                await context.PostAsync("Form returned empty response! Type anything to restart it.");
            }
        }
        catch (OperationCanceledException)
        {
            await context.PostAsync("You canceled the form! Type anything to restart it.");
        }
        string lowMCV = GetValue(patientID, getValueUri, "lowMCV");
        if (lowMCV == "yes")
        {
            System.Threading.Thread.Sleep(2000);
            await context.PostAsync("We need check your serum ferritin. If it is low you might have iron deficiency anemia. Otherwise, we need further workup. For more information about iron deficiency anemia, please visit https://www.nhlbi.nih.gov/health-topics/iron-deficiency-anemia.");
            System.Threading.Thread.Sleep(4000);
            await context.PostAsync("This is the end of the simulated case. Please type 'restart' to start over.");
        } else
        {
            string highMCV = GetValue(patientID, getValueUri, "highMCV");
            if (highMCV == "yes")
            {
                System.Threading.Thread.Sleep(2000);
                await context.PostAsync("We need check your serum folate and vitamin B12 to rule out serum folate or vitamin B12 deficiency anemia. If neither of them is low, we need further workup. For more information about folate and vitamin B12 deficiency anemia, please visit https://medlineplus.gov/ency/article/000551.htm and https://medlineplus.gov/ency/article/000574.htm.");
                System.Threading.Thread.Sleep(4000);
                await context.PostAsync("This is the end of the simulated case. Please type 'restart' to start over.");
            } else if (highMCV == "no")
            {
                System.Threading.Thread.Sleep(2000);
                await context.PostAsync("We need check your serum reticulocyte count which tells us whether red blood cells are produced well or not. If reticulotye count is high, it means red blood cell production is good so we may question red blood cell breakdown or loss. If it is not high while MCV is high as in your case, red blood cell production might have some problem and we need further workup. For more information about reticulocyte count, please visit https://medlineplus.gov/ency/article/003637.htm.");
                System.Threading.Thread.Sleep(4000);
                await context.PostAsync("This is the end of the simulated case. Please type 'restart' to start over.");
            }
        }
    }

    //GetValue
    public static dynamic GetValue (string actorId, string requestUri, string variable)
    {
        using(var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            dynamic requestBody = new ExpandoObject();
            requestBody.actorId = actorId;
            requestBody.variable = variable;
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var response = client.SendAsync(request).Result;
            var stringResponseTask = response.Content.ReadAsStringAsync();
            JObject jObj = JObject.Parse(stringResponseTask.Result);
            var value = (dynamic)jObj["value"];
            return value;
        }
    }

    //SetValue
    public async static Task SetValue (string actorId, string requestUri, string variable, dynamic value, string publish)
    {
        using(var client = new HttpClient())
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            dynamic requestBody = new ExpandoObject();
            requestBody.actorId = actorId;
            requestBody.variable = variable;
            requestBody.value = value;
            requestBody.publish = publish;
            string jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            request.Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            await client.SendAsync(request);
        }
    }
}

