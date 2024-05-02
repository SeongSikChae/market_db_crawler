using System.Diagnostics;
using System.Text;

namespace MarketCrawlerLib.Crawler.Tests
{
    [TestClass]
    public class AuctionCrawlerTests
    {
        [TestMethod]
        public async Task GetCategoriesTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ICrawler crawler = new AuctionCrawler();
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
        public async Task GetCategoriesTest2()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ICrawler crawler = new AuctionCrawler();
            {
                Category? category;
                using (MemoryStream stream = new MemoryStream())
                {
                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(@"{
                        ""CategoryType"": ""Auction"",
                        ""Id"": ""8"",
                        ""Name"": ""브랜드 패션"",
                        ""Level"": 1
                    }");
                    writer.Flush();
                    stream.Position = 0;
                    category = await System.Text.Json.JsonSerializer.DeserializeAsync<Category>(stream);
                }
                Assert.IsNotNull(category);
                List<Category> categories = await crawler.GetCategories(category);
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
            {
                Category? category;
                using (MemoryStream stream = new MemoryStream())
                {
                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(@"{
                        ""CategoryType"": ""Auction"",
                        ""Id"": ""66000000"",
                        ""Name"": ""브랜드 여성의류"",
                        ""Level"": 2
                    }");
                    writer.Flush();
                    stream.Position = 0;
                    category = await System.Text.Json.JsonSerializer.DeserializeAsync<Category>(stream);
                }
                Assert.IsNotNull(category);
                List<Category> categories = await crawler.GetCategories(category);
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
}