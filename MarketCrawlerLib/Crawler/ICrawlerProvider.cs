namespace MarketCrawlerLib.Crawler
{
    public enum CrawlerType
    {
        Auction, Coupang, GMarket, Naver, Street11
    }

    public interface ICrawlerProvider
    {
        ICrawler GetCrawler(CrawlerType type);

        static ICrawlerProvider Default { get; } = new DefaultCrawlerProvider();

        private sealed class DefaultCrawlerProvider : ICrawlerProvider
        {
            public DefaultCrawlerProvider()
            {
                crawlerDic.Add(CrawlerType.Auction, new AuctionCrawler());
                crawlerDic.Add(CrawlerType.Coupang, new CoupangCrawler());
                crawlerDic.Add(CrawlerType.GMarket, new GMarketCrawler());
                crawlerDic.Add(CrawlerType.Naver, new NaverCrawler());
                crawlerDic.Add(CrawlerType.Street11, new Street11Crawler());
            }

            private readonly Dictionary<CrawlerType, ICrawler> crawlerDic = new Dictionary<CrawlerType, ICrawler>();

            public ICrawler GetCrawler(CrawlerType type)
            {
                if (crawlerDic.TryGetValue(type, out ICrawler? crawler))
                    return crawler;
                throw new InvalidOperationException($"not found crawler : {type}");
            }
        }
    }
}
