using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TextExtract
{
    class Program
    {
        const string Api_Key = "9bde2185982740c9852c5d3fef9c178a";
        //const string Api_Location = "https://textextractapi.cognitiveservices.azure.com/";
        static void Main(string[] args)
        {
            var imageToAnalyze = @"E:\ComputerVision\TextExtract\ConsoleApp1\Images\Capture.PNG";
            TextExtract(imageToAnalyze, false);
            Console.ReadLine();
        }
            
        public static void PrintResults(string[] results)
        {
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        private static void TextExtract(string fileName, bool outputByWords = false)
        {
            Task.Run(async () =>
            {
                var result = await TextExtractionCore(fileName, outputByWords);
                PrintResults(result);
            }).Wait();
        }

        private static async Task<string[]> TextExtractionCore(string fileName, bool outputByWords)
        {
            VisionServiceClient client = new VisionServiceClient(Api_Key);
            string[] textResults = null;

            if (File.Exists(fileName))
            {
                using var stream = File.OpenRead(fileName);
                var results = await client.RecognizeTextAsync(stream, "unk", false);
                textResults = GetExtracted(results, outputByWords);
            }

            return textResults;
        }

        private static string[] GetExtracted(OcrResults results, bool outputByWords)
        {
            var items = new List<string>();
            foreach (Region region in results.Regions)
            {
                foreach (Line line in region.Lines)
                {
                    items.Add(GetLineAsString(line));
                }
            }

            return items.ToArray();
        }

        private static string GetLineAsString(Line line)
        {
            var words = GetWords(line);
            return words.Count > 0 ? string.Join(" ", words) : string.Empty;
        }

        private static List<string> GetWords(Line line)
        {
            var words = new List<string>();
            foreach (Word word in line.Words)
            {
                words.Add(word.Text);
            }

            return words;
        }
    }
}
