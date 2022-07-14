using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAIDemo.Services
{
    public class TextToSpeechService
    {
        CancellationTokenSource cts;

        public async void Speak(string text)
        {
            cts = new CancellationTokenSource();

            await TextToSpeech.Default.SpeakAsync(text, cancelToken: cts.Token);
        }
        public void CancelSpeech()
        {
            if (cts?.IsCancellationRequested ?? true)
                return;

            cts.Cancel();
        }


    }
}
