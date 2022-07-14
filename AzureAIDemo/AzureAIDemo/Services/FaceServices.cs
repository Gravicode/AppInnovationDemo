using AzureAIDemo.Data;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Services
{
    public class FaceServices
    {
        // From your Face subscription in the Azure portal, get your subscription key and endpoint.
        string SUBSCRIPTION_KEY;
        string ENDPOINT;
        const string RECOGNITION_MODEL4 = RecognitionModel.Recognition04;
        IFaceClient faceClient { set; get; }
        public FaceServices()
        {
            SUBSCRIPTION_KEY = AppConstants.FaceApiKey;
            ENDPOINT = AppConstants.FaceApiEndPoint;
            // Authenticate.
            faceClient = Authenticate(ENDPOINT, SUBSCRIPTION_KEY);

            
        }
      

        /*
         *	AUTHENTICATE
         *	Uses subscription key and region to create a client.
         */
        public IFaceClient Authenticate(string endpoint, string key)
        {
            return new FaceClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };
        }

        // Detect faces from image url for recognition purpose. This is a helper method for other functions in this quickstart.
        // Parameter `returnFaceId` of `DetectWithUrlAsync` must be set to `true` (by default) for recognition purpose.
        // Parameter `FaceAttributes` is set to include the QualityForRecognition attribute. 
        // Recognition model must be set to recognition_03 or recognition_04 as a result.
        // Result faces with insufficient quality for recognition are filtered out. 
        // The field `faceId` in returned `DetectedFace`s will be used in Face - Find Similar, Face - Verify. and Face - Identify.
        // It will expire 24 hours after the detection call.
        public async Task<List<DetectedFace>> DetectFaceFromUrl(string url, string recognition_model=RECOGNITION_MODEL4)
        {
            try
            {
                // Detect faces from image URL. Since only recognizing, use the recognition model 1.
                // We use detection model 3 because we are not retrieving attributes.
                IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithUrlAsync(url, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection01, returnFaceAttributes: new List<FaceAttributeType> {  FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Emotion });
                List<DetectedFace> sufficientQualityFaces = new List<DetectedFace>();
                foreach (DetectedFace detectedFace in detectedFaces)
                {
                    sufficientQualityFaces.Add(detectedFace);
                    /*
                    var faceQualityForRecognition = detectedFace.FaceAttributes.;
                    if (faceQualityForRecognition.HasValue && (faceQualityForRecognition.Value >= QualityForRecognition.Medium))
                    {
                        sufficientQualityFaces.Add(detectedFace);
                    }*/
                }
                Console.WriteLine($"{detectedFaces.Count} face(s) with {sufficientQualityFaces.Count} having sufficient quality for recognition detected from image `{Path.GetFileName(url)}`");

                return sufficientQualityFaces.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }

        public async Task<List<DetectedFace>> DetectFaceFromBytes(byte[] ImageData, string recognition_model = RECOGNITION_MODEL4)
        {
            try
            {
                // Detect faces from image URL. Since only recognizing, use the recognition model 1.
                // We use detection model 3 because we are not retrieving attributes.
                var memoryStream = new MemoryStream(ImageData);
                IList<DetectedFace> detectedFaces = await faceClient.Face.DetectWithStreamAsync(memoryStream, recognitionModel: recognition_model, detectionModel: DetectionModel.Detection03, returnFaceAttributes: new List<FaceAttributeType> { FaceAttributeType.Age, FaceAttributeType.Gender, FaceAttributeType.Emotion });
                List<DetectedFace> sufficientQualityFaces = new List<DetectedFace>();
                foreach (DetectedFace detectedFace in detectedFaces)
                {
                    /*
                    var faceQualityForRecognition = detectedFace.FaceAttributes.QualityForRecognition;
                    if (faceQualityForRecognition.HasValue && (faceQualityForRecognition.Value >= QualityForRecognition.Medium))
                    {
                        sufficientQualityFaces.Add(detectedFace);
                    }*/
                    sufficientQualityFaces.Add(detectedFace);
                }
                Console.WriteLine($"{detectedFaces.Count} face(s) with {sufficientQualityFaces.Count} having sufficient quality for recognition detected from image bytes");

                return sufficientQualityFaces.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
