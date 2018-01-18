using System;

namespace Xcodo.BetterKnowAFrameworkScraper
{
    public class ShortLinkPage
    {
        public enum LinkResponse { NotTried, Success, NotSet, UnknownError, BadRequestError, NotFoundError, ForbiddenError };

        public LinkResponse Response { get; set; } = LinkResponse.NotTried;

        public int ShowNumber { get; set; }

        public Uri ShortAddress { get; set; }

        public Uri LinkedAddress { get; set; }

        public string LinkedTitle { get; set; }
    }
}
