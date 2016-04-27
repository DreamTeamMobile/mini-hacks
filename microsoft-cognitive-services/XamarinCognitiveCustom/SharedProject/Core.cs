using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SharedProject
{
    public class Core
    {
        private static async Task<Emotion[]> GetHappiness(Stream stream)
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

        //Average happiness calculation in case of multiple people
        public static async Task<float> GetAverageHappinessScore(Stream stream)
        {
            Emotion[] emotionResults = await GetHappiness(stream);

            float score = 0;
            foreach (var emotionResult in emotionResults)
            {
                score = score + emotionResult.Scores.Happiness;
            }

            return score / emotionResults.Count();
        }

        public static async Task<string> GetEmotionProfile(Stream stream)
        {
            Emotion[] emotionResults = await GetHappiness(stream);


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

            string outSting = String.Empty;
            outSting += $"{Math.Round(happinessScore / emotionResults.Count() * 100)}% happy";
            outSting += "\n";
            outSting += $"{Math.Round(sadnessScore / emotionResults.Count() * 100)}% sad";
            outSting += "\n";
            outSting += $"{Math.Round(fearScore / emotionResults.Count() * 100)}% scared";
            outSting += "\n";
            outSting += $"{Math.Round(angerScore / emotionResults.Count() * 100)}% angry";
            outSting += "\n";
            outSting += $"{Math.Round(surpriseScore / emotionResults.Count() * 100)}% surprised";
            return outSting;
        }

        public static string GetHappinessMessage(float score)
        {
            score = score * 100;
            double result = Math.Round(score, 2);

            if (score >= 50)
                return result + " % :-)";
            else
                return result + "% :-(";
        }
    }
}