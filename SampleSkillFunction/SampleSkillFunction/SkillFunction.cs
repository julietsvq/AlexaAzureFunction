using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace SampleSkillFunction
{
    public static class SkillFunction
    {
        [FunctionName("SkillFunction")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            dynamic data = await req.Content.ReadAsAsync<object>();

            if (data.request.type == "IntentRequest")
            {
                string intent = data.request.intent.name;
                System.Random rnd = new System.Random();
                string message;
                int value = 0;

                switch (intent)
                {
                    case "Random":
                        int min, max = 0;                       
                        
                        string value1 = data.request.intent.slots.FirstNumber.value;
                        string value2 = data.request.intent.slots.SecondNumber.value;

                        if (int.TryParse(value1, out min) && int.TryParse(value2, out max))
                        {                           
                            value = rnd.Next(min, max);
                            message = value.ToString();
                        }
                        else
                        {
                            value = rnd.Next(0, 10);
                            message = value.ToString();
                        }

                        return SendAnswer(req, message);

                    case "FlipACoin":
                        value = rnd.Next(2);
                        message = (value == 1) ? "heads" : "tails";
                        return SendAnswer(req, message);

                    case "RollADice":
                        value = rnd.Next(1, 6);
                        message = value.ToString();
                        return SendAnswer(req, message);

                    default:
                        message = "This intent exists in the Alexa Intent Schema, but has not been implemented in your Azure function";
                        return SendAnswer(req, message);
                }
            }

            else if (data) //data.request.type == "LaunchRequest" || "SessionEndedRequest"
            {
                return SendAnswer(req, "No intent was called");
            }

            else
            {
                return SendAnswer(req, "Something went wrong, no data received");
            }
        }

        private static HttpResponseMessage SendAnswer(HttpRequestMessage req, string message)
        {
            return req.CreateResponse(HttpStatusCode.OK, new
            {
                version = "1.0",
                sessionAttributes = new { },
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = message,
                    },
                    shouldEndSession = true
                }
            });
        }
    }
}
