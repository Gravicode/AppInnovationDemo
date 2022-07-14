using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Data
{
    public class AppConstants
    {
        public const string TranslateApiKey = "95b95f99140b4d66ab004d13970e6720";
        public const string TranslateRegion = "southeastasia";
        public const string TranslateEndPoint = "https://api.microsofttranslator.com/v2/http.svc/translate";//"https://api.cognitive.microsofttranslator.com";
        
        public const string LanguageApiKey = "5f502189823a41488d305d9737c85679";
        public const string LanguageApiEndPoint = "https://azureailanguageapi.cognitiveservices.azure.com/";
        
        public const string FaceApiKey = "93a4da170c2a4dffb7383d16f7cab2fa";
        public const string FaceApiEndPoint = "https://azureaiface.cognitiveservices.azure.com/";
        
        public const string SpeechApiKey = "3df0b92152254e6c9dd50944eb164c16";
        public const string SpeechApiRegion = "southeastasia"; 
        
        public const string CVApiKey = "99708302f06f4062a9a55f513793affe";
        public const string CVApiEndPoint = "https://azureaicv.cognitiveservices.azure.com/";
        
        public const string BOTSecret = "wDVIBZkwslU.L7hLC1OGAcv7uoQxDyfNzGkf_Keej8toxsazPK6k7L8";

        public const string FormRecognizerApiKey = "71ff2578e481468b83cfc85dc298191b";
        public const string FormRecognizerEndPoint = "https://azureaiformreader.cognitiveservices.azure.com/";

        public static readonly string AuthenticationTokenEndpoint = "https://southeastasia.api.cognitive.microsoft.com/sts/v1.0";
    }
}
