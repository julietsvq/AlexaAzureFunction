using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace SampleSkillFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            //log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            //string name = req.GetQueryNameValuePairs()
            //    .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            //    .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            if (data.request.type == "IntentRequest")
            {
                string intent = data.request.intent.name;

                switch (intent)
                {
                    case "HelloYou":
                        int min, max = 0;
                        string message;

                        if (int.TryParse(data.request.intent.slots["number1"].value, out min) && int.TryParse(data.request.intent.slots["number2"].value, out max))
                        {
                            System.Random rnd = new System.Random();
                            int value = rnd.Next(min, max);
                            message = value.ToString();
                        }
                        else
                            message = "Wrong number values, please try again";

                        return SendAnswer(req, message);
                    default:
                        return SendAnswer(req, "That's not an intent");
                }
            }

            else //data.request.type == "LaunchRequest" || "SessionEndedRequest"
            {
                return SendAnswer(req, "No intent was called");
            }

            // Set name to query string or body data
            //name = name ?? data?.name;

            //return name == null
            //    ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            //    : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
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
