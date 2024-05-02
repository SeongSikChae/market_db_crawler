using MarketCrawlerLib.Crawler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketCrawlerLibTests.Crawler
{
    [TestClass]
    public class ICrawlerProviderTests
    {
        [TestMethod]
        public async Task GetAuctionCrawlerTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = ICrawlerProvider.Default.GetCrawler(CrawlerType.Auction);
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public async Task GetCoupangCrawlerTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = ICrawlerProvider.Default.GetCrawler(CrawlerType.Coupang);
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public async Task GetGMarketCrawlerTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = ICrawlerProvider.Default.GetCrawler(CrawlerType.GMarket);
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public async Task GetNaverCrawlerTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = ICrawlerProvider.Default.GetCrawler(CrawlerType.Naver);
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public async Task GetStreet11CrawlerTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = ICrawlerProvider.Default.GetCrawler(CrawlerType.Street11);
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }
    }
}
