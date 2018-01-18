using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xcodo.BetterKnowAFrameworkScraper
{
    public static class Scraper
    {
        public static int FirstShortLinkShow { get; } = 1264;

        private static Uri CreateUri(uint showNumber) => new Uri($"http://{showNumber}.pwop.me");

        public static async Task<ShortLinkPage> GetShortLinkPage(int showNumber, HttpClient client)
        {
            if (showNumber < FirstShortLinkShow)
            {
                throw new ArgumentOutOfRangeException(nameof(showNumber), "Show number is earlier than the short links started");
            }

            HttpResponseMessage response = null;
            var page = new ShortLinkPage
            {
                Response = ShortLinkPage.LinkResponse.NotTried,
                ShowNumber = showNumber,
                ShortAddress = CreateUri((uint)showNumber),
            };

            try
            {
                response = await client.GetAsync(page.ShortAddress, HttpCompletionOption.ResponseContentRead);
                response.EnsureSuccessStatusCode();

                page.Response = ShortLinkPage.LinkResponse.Success;
                page.LinkedAddress = response.RequestMessage.RequestUri;
                page.LinkedTitle = GetPageTitle(await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException?.HResult == -2147012889)
                {
                    page.Response = ShortLinkPage.LinkResponse.NotSet;
                }
                else if (response != null)
                {
                    page.LinkedAddress = response?.RequestMessage?.RequestUri;

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            page.Response = ShortLinkPage.LinkResponse.NotFoundError;
                            break;
                        case System.Net.HttpStatusCode.Forbidden:
                            page.Response = ShortLinkPage.LinkResponse.ForbiddenError;
                            break;
                        case System.Net.HttpStatusCode.BadRequest:
                            page.Response = ShortLinkPage.LinkResponse.BadRequestError;
                            break;
                        default:
                            page.Response = ShortLinkPage.LinkResponse.UnknownError;
                            break;
                    }
                }
                else
                {
                    page.Response = ShortLinkPage.LinkResponse.UnknownError;
                }
            }
            catch (Exception ex)
            {
                page.Response = ShortLinkPage.LinkResponse.UnknownError;
                return page;
            }

            return page;
        }

        private static string GetPageTitle(string page)
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
