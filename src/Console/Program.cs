using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xcodo.BetterKnowAFrameworkScraper.Console
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using (var client = new HttpClient())
            {
                for (int i = 1520; i >= 1519; i--)
                {
                    try
                    {
                        var linkedPage = await Scraper.GetShortLinkPage(i, client);
                        string answer = $"{i:00000} = ";
                        switch (linkedPage.Response)
                        {
                            case ShortLinkPage.LinkResponse.Success:
                                answer += $"{linkedPage.LinkedAddress}: {linkedPage.LinkedTitle}";
                                break;
                            case ShortLinkPage.LinkResponse.NotSet:
                                answer += "No link found";
                                break;
                            case ShortLinkPage.LinkResponse.BadRequestError:
                                answer += $"{linkedPage.LinkedAddress}: ERROR 400 Bad request";
                                break;
                            case ShortLinkPage.LinkResponse.ForbiddenError:
                                answer += $"{linkedPage.LinkedAddress}: ERROR 403 Forbidden";
                                break;
                            case ShortLinkPage.LinkResponse.NotFoundError:
                                answer += $"{linkedPage.LinkedAddress}: ERROR 404 Not found";
                                break;
                            default:
                                answer += "UNKNOWN ERROR";
                                break;
                        }

                        await System.Console.Out.WriteLineAsync(answer);
                    }
                    catch (Exception ex)
                    {
                        await System.Console.Out.WriteLineAsync($"{i:00000} EXCEPTION {ex.GetType()}: {ex.Message}");
                    }
                    
                }
            }
        }

        
    }
}
