﻿using System.Diagnostics;
using System.Text;

namespace MarketCrawlerLib.Crawler.Tests
{
    [TestClass]
    public class CoupangCrawlerTests
    {
        [TestMethod]
        public async Task GetCategoriesTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ICrawler crawler = new CoupangCrawler();
            List<Category> categories = await crawler.GetCategories();
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync(stream, categories.Cast<CoupangCategory>(), new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }
    }
}