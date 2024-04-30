using System.Diagnostics;
using System.Text;

namespace MarketCrawlerLib.Crawler.Tests
{
    [TestClass]
    public class Street11CrawlerTests
    {
        [TestMethod]
        public async Task CategoriesTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ICrawler crawler = new Street11Crawler();
            List<Category> categories = await crawler.Categories("pageId=SIDEMENU_V3");
            using (MemoryStream stream = new MemoryStream())
            {
                await System.Text.Json.JsonSerializer.SerializeAsync<List<Category>>(stream, categories, new System.Text.Json.JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                stream.Position = 0;
                using StreamReader reader = new StreamReader(stream);
                Trace.WriteLine(reader.ReadToEnd());
            }
        }

        [TestMethod]
        public async Task SubCategoreisTest()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ICrawler crawler = new Street11Crawler();
            {
                Category? category;
                using (MemoryStream stream = new MemoryStream())
                {
                    using StreamWriter writer = new StreamWriter(stream);
                    writer.Write(@"{
                        ""Id"": ""163122"",
                        ""Name"": ""브랜드패션"",
                        ""IsSubCategory"": false,
                        ""Link"": ""pageId=META163134&metaLctgrNo=163122&metaMctgrNo=163134""
                    }");
                    writer.Flush();
                    stream.Position = 0;
                    category = await System.Text.Json.JsonSerializer.DeserializeAsync<Category>(stream);
                }
                Assert.IsNotNull(category);
                List<Category> categories = await crawler.Categories(category);
                using (MemoryStream stream = new MemoryStream())
                {
                    await System.Text.Json.JsonSerializer.SerializeAsync<List<Category>>(stream, categories, new System.Text.Json.JsonSerializerOptions
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
                        ""Id"": ""1149695"",
                        ""Name"": ""컴퓨터"",
                        ""IsSubCategory"": true,
                        ""Link"": ""metaCtgrNo=1149695""
                    }");
                    writer.Flush();
                    stream.Position = 0;
                    category = await System.Text.Json.JsonSerializer.DeserializeAsync<Category>(stream);
                }
                Assert.IsNotNull(category);
                List<Category> categories = await crawler.Categories(category);
                using (MemoryStream stream = new MemoryStream())
                {
                    await System.Text.Json.JsonSerializer.SerializeAsync<List<Category>>(stream, categories, new System.Text.Json.JsonSerializerOptions
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