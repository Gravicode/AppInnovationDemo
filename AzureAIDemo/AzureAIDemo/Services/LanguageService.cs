using Azure;
using Azure.AI.TextAnalytics;
using AzureAIDemo.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Services
{
    public class LanguageService
    {
        private AzureKeyCredential credentials;
        private Uri endpoint;
        TextAnalyticsClient client;
        public LanguageService()
        {
            credentials = new AzureKeyCredential(AppConstants.LanguageApiKey);
            endpoint = new Uri(AppConstants.LanguageApiEndPoint);
            client = new TextAnalyticsClient(endpoint, credentials);

        }

        // Example method for summarizing text
        public async Task<List<SummarySentence>> SummarizeText(string document)
        {
            try
            {
                // Prepare analyze operation input. You can add multiple documents to this list and perform the same
                // operation to all of them.
                var batchInput = new List<string>
            {
                document
            };

                TextAnalyticsActions actions = new TextAnalyticsActions()
                {
                    ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction() }
                };

                // Start analysis process.
                AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions);
                await operation.WaitForCompletionAsync();
                // View operation status.
                //Console.WriteLine($"AnalyzeActions operation has completed");
                //Console.WriteLine();
                //
                //Console.WriteLine($"Created On   : {operation.CreatedOn}");
                //Console.WriteLine($"Expires On   : {operation.ExpiresOn}");
                //Console.WriteLine($"Id           : {operation.Id}");
                //Console.WriteLine($"Status       : {operation.Status}");
                //
                //Console.WriteLine();

                var result = new List<SummarySentence>();
                // View operation results.
                await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
                {
                    IReadOnlyCollection<ExtractSummaryActionResult> summaryResults = documentsInPage.ExtractSummaryResults;

                    foreach (ExtractSummaryActionResult summaryActionResults in summaryResults)
                    {
                        if (summaryActionResults.HasError)
                        {
                            Console.WriteLine($"  Error!");
                            Console.WriteLine($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
                            Console.WriteLine($"  Message: {summaryActionResults.Error.Message}");
                            continue;
                        }

                        foreach (ExtractSummaryResult documentResults in summaryActionResults.DocumentsResults)
                        {
                            if (documentResults.HasError)
                            {
                                Console.WriteLine($"  Error!");
                                Console.WriteLine($"  Document error code: {documentResults.Error.ErrorCode}.");
                                Console.WriteLine($"  Message: {documentResults.Error.Message}");
                                continue;
                            }

                            Console.WriteLine($"  Extracted the following {documentResults.Sentences.Count} sentence(s):");
                            Console.WriteLine();

                            foreach (SummarySentence sentence in documentResults.Sentences)
                            {
                                result.Add(sentence);
                                //Console.WriteLine($"  Sentence: {sentence.Text}");
                                //Console.WriteLine();
                            }
                        }
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }
        // Example method for detecting sentiment from text. 
        public async Task<DocumentSentiment> SentimentAnalysis(string text)
        {
            DocumentSentiment documentSentiment = await client.AnalyzeSentimentAsync(text);
            Console.WriteLine($"Document sentiment: {documentSentiment.Sentiment}\n");
            return documentSentiment;
            /*
            foreach (var sentence in documentSentiment.Sentences)
            {
                Console.WriteLine($"\tText: \"{sentence.Text}\"");
                Console.WriteLine($"\tSentence sentiment: {sentence.Sentiment}");
                Console.WriteLine($"\tPositive score: {sentence.ConfidenceScores.Positive:0.00}");
                Console.WriteLine($"\tNegative score: {sentence.ConfidenceScores.Negative:0.00}");
                Console.WriteLine($"\tNeutral score: {sentence.ConfidenceScores.Neutral:0.00}\n");
            }*/
        }
        public async Task<PiiEntityCollection> ExtractPII(string document)
        {
            PiiEntityCollection entities = await client.RecognizePiiEntitiesAsync(document);
            /*
            Console.WriteLine($"Redacted Text: {entities.RedactedText}");
            if (entities.Count > 0)
            {
                Console.WriteLine($"Recognized {entities.Count} PII entit{(entities.Count > 1 ? "ies" : "y")}:");
                foreach (PiiEntity entity in entities)
                {
                    Console.WriteLine($"Text: {entity.Text}, Category: {entity.Category}, SubCategory: {entity.SubCategory}, Confidence score: {entity.ConfidenceScore}");
                }
            }
            else
            {
                Console.WriteLine("No entities were found.");
            }*/
            return entities;
        }


    }
}
