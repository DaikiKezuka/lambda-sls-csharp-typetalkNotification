using System;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Typetalk
{
    class Handler
    {
        public void Notification(EventData input, ILambdaContext context)
        {
            if (null == input)
            {
                return;
            }

            PostWithTypetalkToken(input?.Records[0]).Wait();

            Console.WriteLine("PostWithTypetalkToken end.");
        }

        public async Task PostWithTypetalkToken(Record record)
        {
            if (await ValidateHealthUrl(record))
            {
                return;
            }

            var typetalkToken = "xxxxxxxxxxxxxxxxxxxx";
            var topicId = 12345; //use your topic id

            var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {
                    "message",
                    $"EventSubscriptionArn : {record.EventSubscriptionArn}{Environment.NewLine}" +
                    $"Subject : {record.Sns.Subject}"
                }
            });

            var client = new HttpClient();
            await client.PostAsync(
                "https://typetalk.com/api/v1/topics/" + topicId + $"?typetalkToken={typetalkToken}",
                 content);
        }
        public async Task<bool> ValidateHealthUrl(Record record)
        {
            var result = true;

            var url = $"https://HealthURL";

            var client = new HttpClient();
            var response = await client.GetAsync(url);

            Console.WriteLine($"HealthURL({url}) StatusCode is {response.StatusCode.ToString()}.");

            if (response.StatusCode.ToString() != "200")
            {
                result = false;
            }

            return result;
        }

        public class EventData
        {
            public Record[] Records { get; set; }
        }

        public class Record
        {
            public string EventSubscriptionArn { get; set; }
            public Sns Sns { get; set; }
        }

        public class Sns
        {
            public string Subject { get; set; }
        }
    }
}
