using System;
using System.Dynamic;
using System.Text.Json;

namespace blazor.codex.Models
{
    public class ParamCompletions
    {
        public string prompt { get; set; }
        public int temperature { get; set; }
        public int max_tokens { get; set; }
        public string stop { get; set; }
        public int n { get; set; }
    }
    public class CodexEngine
    {
        const int maxPromptLength = 3200;

        public Context context { get; set; }
        public ContentFiltering detectSensitiveContent { get; set; }
        HttpClient client;

        public CodexEngine()
        {
            detectSensitiveContent = new ContentFiltering();
            context = new Context(BaseContext.baseContext);
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("OpenAI-Organization", AppConstants.OPENAI_ORGANIZATION_ID);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {AppConstants.OPENAI_API_KEY}");

        }

        public async Task<dynamic> getCompletion(string command)
        {
            try
            {
                var prompt = context.getPrompt(command);

                if (prompt.Length > maxPromptLength)
                {
                    context.trimContext(maxPromptLength - command.Length + 6); // The max length of the prompt, including the command, comment operators and spacing.
                }


                var url = $"https://api.openai.com/v1/engines/{AppConstants.OPENAI_ENGINE_ID}/completions";

                var param = new ParamCompletions() { prompt=prompt,
            max_tokens= 800,
            temperature= 0,
            stop= "/*",
            n= 1 };
                var json = JsonSerializer.Serialize(param);
                var res = await client.PostAsync(url, new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                if (res.IsSuccessStatusCode)
                {
                    var data = await res.Content.ReadAsStringAsync();
                    
                    dynamic obj = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
                    string code = obj.choices[0].text;


                    var sensitiveContentFlag = await detectSensitiveContent.detectSensitiveContent(command + "\n" + code);

                    // The flag can be 0, 1 or 2, corresponding to 'safe', 'sensitive' and 'unsafe'
                    if (sensitiveContentFlag > 0)
                    {
                        Console.WriteLine(
                            sensitiveContentFlag == 1
                            ? "Your message or the model's response may have contained sensitive content."
                            : "Your message or the model's response may have contained unsafe content."
                        );

                        code = String.Empty;
                    }
                    else
                    {
                        //only allow safe interactions to be added to the context history
                        context.addInteraction(command, code);
                    }

                    return new {
                        code,
                        prompt,
                        sensitiveContentFlag
                    };


                }
                else
                {
                        //throw new Error(`${response.status} ${response.statusText}`);
                        var error = "There is an issue with your OpenAI credentials, please check your OpenAI API key, organization ID and model name.Modify the credentials and restart the server!";
                        if (res.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                          Console.WriteLine(error);
                        }
                        return new { error };
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return new { };
        }
    }
}

/*
 require("dotenv").config();
import fetch from "isomorphic-fetch";

// Contains the helper methods for interacting with Codex and crafting model prompts
import { baseContext } from "./contexts/context1";
import Context from "./Context";
import { detectSensitiveContent } from "./contentFiltering";

const maxPromptLength = 3200;

// CURRENTLY SINGLE TENANT - WOULD NEED TO UPDATE THIS TO A MAP OF TENANT IDs TO PROMPTS TO MAKE MULTI-TENANT
export const context = new Context(baseContext);

export async function getCompletion(command: string) {      
    let prompt = context.getPrompt(command);

    if (prompt.length > maxPromptLength) {
        context.trimContext(maxPromptLength - command.length + 6); // The max length of the prompt, including the command, comment operators and spacing.
    }

    // To learn more about making requests to OpanAI API, please refer to https://beta.openai.com/docs/api-reference/making-requests.
    // Here we use the following endpoint pattern for engine selection.
    // https://api.openai.com/v1/engines/{engine_id}/completions
    // You can switch to different engines that are available to you. Learn more about engines - https://beta.openai.com/docs/engines/engines
    const response = await fetch(`https://api.openai.com/v1/engines/${process.env.OPENAI_ENGINE_ID}/completions`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${process.env.OPENAI_API_KEY}`,
            "OpenAI-Organization": `${process.env.OPENAI_ORGANIZATION_ID}`
        },
        body: JSON.stringify({
            prompt,
            max_tokens: 800,
            temperature: 0,
            stop: "/*",
            n: 1
        })
    });

    // catch errors
    if (!response.ok) {
        //throw new Error(`${response.status} ${response.statusText}`);
        const error = `There is an issue with your OpenAI credentials, please check your OpenAI API key, organization ID and model name. Modify the credentials and restart the server!`;
        if (response.status == 404){
            console.log(error);
        }
        return {error};
    }

    const json = await response.json();
    let code = json.choices[0].text;

    let sensitiveContentFlag = await detectSensitiveContent(command + "\n" + code);

    // The flag can be 0, 1 or 2, corresponding to 'safe', 'sensitive' and 'unsafe'
    if (sensitiveContentFlag > 0) {
        console.warn(
            sensitiveContentFlag === 1
            ? "Your message or the model's response may have contained sensitive content."
            : "Your message or the model's response may have contained unsafe content."
        );

        code = '';
    }
    else {
        //only allow safe interactions to be added to the context history
        context.addInteraction(command, code);
    }    

    return {
        code,
        prompt,
        sensitiveContentFlag
    };
}

*/