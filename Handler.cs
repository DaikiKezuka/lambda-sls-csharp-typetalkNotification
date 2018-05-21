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
        public static CodepipeLine message;
        public void Notification(CodepipeLine request, ILambdaContext context)
        {
            message = request;
            PostWithTypetalkToken().Wait();
            Console.WriteLine("PostWithTypetalkToken end.");
        }

        public async Task PostWithTypetalkToken()
        {
            var typetalkToken = "xxxxxxxxxxxxxxxxxxxx";
            var topicId = 12345; //use your topic id

            var content = new FormUrlEncodedContent(new Dictionary<string, string>() {
                {
                     "message",
                     $"{message.source} is {message.detail.status}.{Environment.NewLine}" +
                     $"Pipeline Name : {message.detail.pipeline}{Environment.NewLine}" +
                     $"Pipeline Stage : {message.detail.stage}" }
            });

            var client = new HttpClient();
            await client.PostAsync(
                "https://typetalk.com/api/v1/topics/" + topicId + $"?typetalkToken={typetalkToken}",
                 content);
        }

        public class CodepipeLine
        {
            public string source { get; set; }
            public detail detail { get; set; }
        }

        public class detail
        {
            public string pipeline { get; set; }

            public string stage { get; set; }

            public string status { get; set; }
        }
    }
}
