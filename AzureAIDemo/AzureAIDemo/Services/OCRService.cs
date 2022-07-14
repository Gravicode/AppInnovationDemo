using AzureAIDemo.Data;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Services
{
    internal class OCRService
    {  // Add your Computer Vision subscription key and endpoint
        static string subscriptionKey ;
        static string endpoint ;

        ComputerVisionClient client;
        public OCRService()
        {
            subscriptionKey = AppConstants.CVApiKey;
            endpoint = AppConstants.CVApiEndPoint;

            client = Authenticate(endpoint, subscriptionKey);

        }


        public ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public async Task<IList<ReadResult>> ReadFileUrl(string urlFile)
        {
            try
            {
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine("READ FILE FROM URL");
                Console.WriteLine();

                // Read text from URL
                var textHeaders = await client.ReadAsync(urlFile);
                // After the request, get the operation location (operation ID)
                string operationLocation = textHeaders.OperationLocation;
                Thread.Sleep(2000);

                // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
                // We only need the ID and not the full URL
                const int numberOfCharsInOperationId = 36;
                string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

                // Extract the text
                ReadOperationResult results;
                Console.WriteLine($"Extracting text from URL file {Path.GetFileName(urlFile)}...");
                Console.WriteLine();
                do
                {
                    results = await client.GetReadResultAsync(Guid.Parse(operationId));
                }
                while ((results.Status == OperationStatusCodes.Running ||
                    results.Status == OperationStatusCodes.NotStarted));

                // Display the found text.
                Console.WriteLine();
                var textUrlFileResults = results.AnalyzeResult.ReadResults;
                foreach (ReadResult page in textUrlFileResults)
                {
                    foreach (Line line in page.Lines)
                    {
                        Console.WriteLine(line.Text);
                    }
                }
                Console.WriteLine();
                return textUrlFileResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }

        public async Task<IList<ReadResult>> ReadImageBytes(byte[] ImageData)
        {
            try { 
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("READ FILE FROM URL");
            Console.WriteLine();

            // Read text from URL
            var stream = new MemoryStream(ImageData);
            var textHeaders = await client.ReadInStreamAsync(stream);
            // After the request, get the operation location (operation ID)
            string operationLocation = textHeaders.OperationLocation;
            Thread.Sleep(2000);

            // Retrieve the URI where the extracted text will be stored from the Operation-Location header.
            // We only need the ID and not the full URL
            const int numberOfCharsInOperationId = 36;
            string operationId = operationLocation.Substring(operationLocation.Length - numberOfCharsInOperationId);

            // Extract the text
            ReadOperationResult results;
            Console.WriteLine($"Extracting text from byte file...");
            Console.WriteLine();
            do
            {
                results = await client.GetReadResultAsync(Guid.Parse(operationId));
            }
            while ((results.Status == OperationStatusCodes.Running ||
                results.Status == OperationStatusCodes.NotStarted));

            // Display the found text.
            Console.WriteLine();
            var textUrlFileResults = results.AnalyzeResult.ReadResults;
            foreach (ReadResult page in textUrlFileResults)
            {
                foreach (Line line in page.Lines)
                {
                    Console.WriteLine(line.Text);
                }
            }
                Console.WriteLine(); 
                return textUrlFileResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            return default;
        }
    }
}
