using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmotionLibrary
{
    public class Core
    {
        private static async Task<Emotion[]> GetEmotions(Stream stream)
        {
            string emotionKey = "f2c3c98c97964b28a75b1a13c278ea2e";

            EmotionServiceClient emotionClient = new EmotionServiceClient(emotionKey);
            var emotionResults = await emotionClient.RecognizeAsync(stream);

            if (emotionResults == null || emotionResults.Count() == 0)
            {
                throw new Exception("Can't detect face");
            }

            return emotionResults;
        }

        public static async Task<string> GetEmotionProfile(Stream stream)
        {
            Emotion[] emotionResults = await GetEmotions(stream);


            float happinessScore = 0;
            float fearScore = 0;
            float angerScore = 0;
            float surpriseScore = 0;
            float sadnessScore = 0;

            
            foreach (var emotionResult in emotionResults)
            {
                happinessScore += emotionResult.Scores.Happiness;
                sadnessScore += emotionResult.Scores.Sadness;
                fearScore += emotionResult.Scores.Fear;
                angerScore += emotionResult.Scores.Anger;
                surpriseScore += emotionResult.Scores.Surprise;
            }

            StringBuilder resultString = new StringBuilder();
            resultString.AppendLine($"{Math.Round(happinessScore/emotionResults.Count()*100)}% happy");
            resultString.AppendLine($"{Math.Round(sadnessScore / emotionResults.Count() * 100)}% sad");
            resultString.AppendLine($"{Math.Round(fearScore / emotionResults.Count() * 100)}% scared");
            resultString.AppendLine($"{Math.Round(angerScore / emotionResults.Count() * 100)}% angry");
            resultString.AppendLine($"{Math.Round(surpriseScore / emotionResults.Count() * 100)}% surprised");
            return resultString.ToString();
        }
    }
}
