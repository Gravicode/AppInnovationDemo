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
    public class ComputerVisionService
    { // Add your Computer Vision subscription key and endpoint
        string subscriptionKey ;
        string endpoint ;
        ComputerVisionClient client;
        public ComputerVisionService()
        {
            endpoint = AppConstants.CVApiEndPoint;
            subscriptionKey = AppConstants.CVApiKey;
            // Create a client
            client = Authenticate(endpoint, subscriptionKey);

        }
     

        /*
         * AUTHENTICATE
         * Creates a Computer Vision client used by each example.
         */
        public  ComputerVisionClient Authenticate(string endpoint, string key)
        {
            ComputerVisionClient client =
              new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
              { Endpoint = endpoint };
            return client;
        }

        public async Task<ImageAnalysis> AnalyzeImageUrl(string imageUrl)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - URL");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags
            };

            Console.WriteLine($"Analyzing the image {Path.GetFileName(imageUrl)}...");
            Console.WriteLine();
            // Analyze the URL image 
            ImageAnalysis results = await client.AnalyzeImageAsync(imageUrl, visualFeatures: features);

            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            return results;
        } 
        
        public async Task<ImageAnalysis> AnalyzeImageBytes(byte[] imageData)
        {
            Console.WriteLine("----------------------------------------------------------");
            Console.WriteLine("ANALYZE IMAGE - stream");
            Console.WriteLine();

            // Creating a list that defines the features to be extracted from the image. 

            List<VisualFeatureTypes?> features = new List<VisualFeatureTypes?>()
            {
                VisualFeatureTypes.Tags, VisualFeatureTypes.Faces, VisualFeatureTypes.Objects, VisualFeatureTypes.Description
            };

            Console.WriteLine($"Analyzing the image ..");
            Console.WriteLine();
            // Analyze the URL image 
            var imgBytes = new MemoryStream(imageData);
            ImageAnalysis results = await client.AnalyzeImageInStreamAsync(imgBytes, visualFeatures: features);

            // Image tags and their confidence score
            Console.WriteLine("Tags:");
            foreach (var tag in results.Tags)
            {
                Console.WriteLine($"{tag.Name} {tag.Confidence}");
            }
            return results;
        }
    }
}
