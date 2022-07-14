using AzureAIDemo.Data;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Services
{
    public enum TranscribeMode { Start, Stop }
    public class RecognizedEventArgs : EventArgs
    {
        public string Text { get; set; }
    }
    
    public class StateChangedEventArgs : EventArgs
    {
        public TranscribeMode State { set; get;}
    }
    public class SpeechService
    {
        string CognitiveServicesApiKey = "YOUR_KEY_GOES_HERE";
        string CognitiveServicesRegion = "westus";
        SpeechRecognizer recognizer;
        public EventHandler<RecognizedEventArgs> Recognized;
        public EventHandler<StateChangedEventArgs> StateChanged;
        public SpeechService()
        {
            CognitiveServicesApiKey = AppConstants.SpeechApiKey;
            CognitiveServicesRegion = AppConstants.SpeechApiRegion;

            // initialize speech recognizer 
            if (recognizer == null)
            {
                var config = SpeechConfig.FromSubscription(CognitiveServicesApiKey, CognitiveServicesRegion);
                recognizer = new SpeechRecognizer(config);
                recognizer.Recognized += (obj, args) =>
                {
                    Recognized?.Invoke(this, new RecognizedEventArgs() { Text = args.Result.Text });
                };


            }

        }
        bool isTranscribing = false;
        public async Task StartTranscribe()
        {
            var status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status == PermissionStatus.Granted)
            {
                // if already transcribing, stop speech recognizer
                if (isTranscribing)
                {
                    try
                    {
                        await recognizer.StopContinuousRecognitionAsync();
                    }
                    catch (Exception ex)
                    {
                        UpdateTranscription(ex.Message);
                    }
                    isTranscribing = false;
                }

                // if not transcribing, start speech recognizer
                else
                {

                    InsertDateTimeRecord();
                    try
                    {
                        await recognizer.StartContinuousRecognitionAsync();
                    }
                    catch (Exception ex)
                    {
                        UpdateTranscription(ex.Message);
                    }
                    isTranscribing = true;
                }
                UpdateDisplayState();
            }
        }
        void UpdateTranscription(string newText)
        {

            if (!string.IsNullOrWhiteSpace(newText))
            {
                Recognized?.Invoke(this, new RecognizedEventArgs() { Text = $"{newText}\n" });
            }

        }

        void InsertDateTimeRecord()
        {
            var msg = $"\n{DateTime.Now.ToString()}:\n";
            UpdateTranscription(msg);
        }

        void UpdateDisplayState()
        {

            if (isTranscribing)
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs() { State = TranscribeMode.Start });
            }
            else
            {
                StateChanged?.Invoke(this, new StateChangedEventArgs() { State = TranscribeMode.Stop });
            }

        }

    }
}
