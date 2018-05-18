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
        public void Notification()
    	{
        	PostWithClientCredentials().Wait();
			Console.WriteLine("PostWithClientCredentials end.");

			PostWithTypetalkToken().Wait();
			Console.WriteLine("PostWithTypetalkToken end.");
    	}

		public async Task PostWithClientCredentials ()
    	{
        	var clientId = "xxxxxxxxxxxxxxxxxxxx";
        	var clientSecret = "xxxxxxxxxxxxxxxxxxxx";
        	var topicId = 12345; //use your topic id
        	var client = new HttpClient();

        	var content = new FormUrlEncodedContent(new Dictionary<string,string>() {
            	{ "client_id", clientId },
            	{ "client_secret", clientSecret },
            	{ "grant_type", "client_credentials" },
            	{ "scope", "topic.post" }
        	});

			await client.PostAsync("https://typetalk.com/oauth2/access_token", content).ContinueWith(res =>
			{
				var str = res.Result.Content.ReadAsStringAsync().Result;
				var dic = JsonConvert.DeserializeObject<Dictionary<string,string>>(str);
				var accessToken = dic["access_token"];

				client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
				return client.PostAsync("https://typetalk.com/api/v1/topics/" + topicId,
					new FormUrlEncodedContent(new Dictionary<string,string>() { { "message", "Hello, Typetalk!" } })).Result;
			});
		}

		public async Task PostWithTypetalkToken()
    	{
        	var typetalkToken = "xxxxxxxxxxxxxxxxxxxx";
			var topicId = 12345; //use your topic id

        	var content = new FormUrlEncodedContent(new Dictionary<string,string>() {
            	{ "message", "Hello, Typetalk!" }
        	});

			var client = new HttpClient();
			await client.PostAsync(
				"https://typetalk.com/api/v1/topics/" + topicId + $"?typetalkToken={typetalkToken}",
				 content);
		}
    }
}
