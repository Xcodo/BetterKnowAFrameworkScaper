using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xcodo.BetterKnowAFrameworkScraper.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            for (int i = 1550; i >= 1400; i--)
            {
                var request = $"http://{i}.pwop.me";
                var (responseUri, responseTitle) = await GetLastUri(request);
                await System.Console.Out.WriteLineAsync($"{i:00000} = {responseUri}: {responseTitle}");
            }
        }

        static async Task<(string, string)> GetLastUri(string uri)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(uri, HttpCompletionOption.ResponseContentRead);
                    response.EnsureSuccessStatusCode();
                    string responseUri = response.RequestMessage.RequestUri.ToString();
                    string responseTitle = GetPageTitle(await response.Content.ReadAsStringAsync());
                    return (responseUri, responseTitle);
                }
                catch
                {
                    return (null, null);
                }
            }
        }

        static string GetPageTitle(string page)
        {
            var m = Regex.Match(page, @"<title>\s*(.+?)\s*</title>");
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            else
            {
                return "";
            }
        }
    }
}
