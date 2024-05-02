using System.Diagnostics;
using System.Text;

namespace MarketCrawlerLib.Crawler.Tests
{
    [TestClass]
    public class NaverCrawlerTests
    {
        [TestMethod]
        public async Task GetCategoriesTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ICrawler<NaverCategory> crawler = new NaverCrawler();
            List<NaverCategory> categories = await crawler.GetCategories();
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