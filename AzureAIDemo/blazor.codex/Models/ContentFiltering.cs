using System.Dynamic;
using System.Text.Json;

namespace blazor.codex
{
    public class ParamContentSensitive
    {
        public string prompt { get; set; }
        public int temperature { get; set; }
        public int max_tokens { get; set; }
        public int top_p { get; set; }
        public int logprobs { get; set; }
    }
    public class ContentFiltering
    {
        HttpClient client;

        public ContentFiltering()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("OpenAI-Organization", AppConstants.OPENAI_ORGANIZATION_ID);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConstants.OPENAI_API_KEY}");
        }
        public async Task<int> detectSensitiveContent(string content)
        {
            try
            {
                var url = "https://api.openai.com/v1/engines/content-filter-alpha/completions";
                
                var param = new ParamContentSensitive() { prompt = $"<|endoftext|>[{content}]\n--\nLabel:", logprobs = 10, top_p = 0, max_tokens = 1, temperature = 0 };
                var json = JsonSerializer.Serialize(param);
                var res = await client.PostAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                    string filterFlag = obj.choices[0].text;
                    return int.Parse(filterFlag);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return default;
        }
        /*
        export async function detectSensitiveContent(content: string): Promise<number> {
        const response = await fetch('https://api.openai.com/v1/engines/content-filter-alpha/completions', {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${process.env.OPENAI_API_KEY}`,
                "OpenAI-Organization": `${process.env.OPENAI_ORGANIZATION_ID}`
            },
            body: JSON.stringify({
                prompt: `<|endoftext|>[${content}]\n--\nLabel:`,
                temperature: 0,
                max_tokens: 1,
                top_p: 0,
                logprobs: 10,
            })
        });

        var json = await response.json();

        const filterFlag = json.choices[0].text as string;
        return parseInt(filterFlag);
    }
        */
    }
}