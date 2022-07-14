using AzureAIDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzureAIDemo.Services
{
    public class TranslateResult
    {
        public TranslateResultArray[] Property1 { get; set; }
    }

    public class TranslateResultArray
    {
        public Detectedlanguage detectedLanguage { get; set; }
        public Translation[] translations { get; set; }
    }

    public class Detectedlanguage
    {
        public string language { get; set; }
        public float score { get; set; }
    }

    public class Translation
    {
        public string text { get; set; }
        public string to { get; set; }
    }

    public class TranslateService
    {
        string subscriptionKey;
        HttpClient httpClient;
        string apiKey;
        string BearerToken;
        DateTime LastGenerateToken = DateTime.MinValue;
        public TranslateService()
        {
            apiKey = AppConstants.TranslateApiKey;
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
        }

        public async Task<TranslateResult> Translate(string textToTranslate, string[] To)
        {
            // Input and output languages are defined as parameters.
            string Tos = string.Empty;
            To.ToList().ForEach(x => Tos += $"&to={x}");
            string route = $"/translate?api-version=3.0&from=en{Tos}";
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonSerializer.Serialize(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(AppConstants.TranslateEndPoint + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", apiKey);

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                var obj = JsonSerializer.Deserialize<TranslateResult>(result);
                return obj;
            }
        }
        
        async Task<string> FetchTokenAsync(string fetchUri)
        {
            UriBuilder uriBuilder = new UriBuilder(fetchUri);
            uriBuilder.Path += "/issueToken";
            var result = await httpClient.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
            BearerToken = await result.Content.ReadAsStringAsync();
            return BearerToken;
        }
        public async Task<string> TranslateTextAsync(string text,string To)
        {
            string requestUri = GenerateRequestUri(AppConstants.TranslateEndPoint, text, To);
            var gap = DateTime.Now - LastGenerateToken;
            if (gap.TotalMinutes > 9)
            {
                BearerToken = await FetchTokenAsync(AppConstants.AuthenticationTokenEndpoint);
                LastGenerateToken = DateTime.Now;
            }
            var response = await SendRequestAsync(requestUri, BearerToken);
            var xml = XDocument.Parse(response);
            return xml.Root.Value;
        }
        string GenerateRequestUri(string endpoint, string text, string to)
        {
            string requestUri = endpoint;
            requestUri += string.Format("?text={0}", Uri.EscapeUriString(text));
            requestUri += string.Format("&to={0}", to);
            return requestUri;
        }
        async Task<string> SendRequestAsync(string url, string bearerToken)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        
    }
}
