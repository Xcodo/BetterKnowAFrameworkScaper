using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xcodo.BetterKnowAFrameworkScraper;

namespace BetterKnowAFrameworkViewer
{
    public class ScrapeManager
    {
        public bool SetupComplete { get; protected set; } = false;

        public ScrapedData Data { get; set; }

        private string dataFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BetterKnowAFrameworkViewer.json");
        
        public async Task LoadDataFromFile()
        {
            if (!File.Exists(dataFilePath))
            {
                Data = new ScrapedData();
            }
            else
            {
                using (var dataStream = new MemoryStream(await File.ReadAllBytesAsync(dataFilePath)))
                using (var streamReader = new StreamReader(dataStream))
                using (var reader = new JsonTextReader(streamReader))
                {
                    try
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        Data = serializer.Deserialize<ScrapedData>(reader);
                    }
                    catch (JsonException)
                    {
                        Data = new ScrapedData();
                    }
                }
            }

            SetupComplete = true;
        }

        public void ClearData() => Data = new ScrapedData();

        public async Task SaveDataToFile()
        {
            using (var writer = new StreamWriter(dataFilePath, false))
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(Data);
                await writer.WriteAsync(contentsToWriteToFile);
            }
        }

        public async Task LoadFromWeb(int fromShowNumber, int toShowNumber)
        {
            Data = new ScrapedData();
            await AddFromWeb(fromShowNumber, toShowNumber);
        }

        public async Task AddFromWeb(int fromShowNumber, int toShowNumber)
        {
            using (var client = new HttpClient())
            {
                int from = (fromShowNumber > toShowNumber) ? fromShowNumber : toShowNumber;
                int to = (fromShowNumber > toShowNumber) ? toShowNumber : fromShowNumber;


                int i = from;
                int numberOfRequests = 10;
                while (i >= to)
                {
                    var shows = new List<int>();
                    for (int j = i; j > (i - numberOfRequests); j--)
                    {
                        if (j >= to)
                        {
                            shows.Add(j);
                        }
                    }

                    await Task.WhenAll(shows.Select(async show => Data.BetterKnowData.Add(await Scraper.GetShortLinkPage(show, client))));

                    i -= numberOfRequests;
                }

                Data.BetterKnowData.Sort((x, y) => y.ShowNumber.CompareTo(x.ShowNumber));
            }
        }
    }
}
