using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public sealed class NaverCategory : Category
    {
        public List<Category> SubCategories { get; set; } = new List<Category>();
    }

    public sealed class NaverCrawler : ICrawler
    {
        private const string UserAgentHeaderName = "User-Agent";
        private const string UserAgentHeaderValue = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36";
        private const string JSON_SCRIPT_ELEMENT_HEADER = "window.__PRELOADED_STATE__=";

        public async Task<List<Category>> GetCategories()
        {
            List<Category> categories = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://m.shopping.naver.com");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            string? json = await client.GetStringAsync("menu?vertical=HOME");
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            HtmlParser parser = new HtmlParser();
            IHtmlDocument doc = await parser.ParseDocumentAsync(json, CancellationToken.None);
            IHtmlElement? body = doc.Body;
            if (body is null)
                return categories;
            IHtmlCollection<IElement> scriptCollection = body.GetElementsByTagName("script");
            IElement? scriptElement = scriptCollection.Where(e => e.InnerHtml.StartsWith(JSON_SCRIPT_ELEMENT_HEADER)).FirstOrDefault();
            if (scriptElement is null)
                return categories;
            json = scriptElement.InnerHtml.Substring(JSON_SCRIPT_ELEMENT_HEADER.Length);
            CategoryQueryResult? result = JsonSerializer.Deserialize<CategoryQueryResult>(json);
            if (result is null)
                return categories;
            if (result.Data is null)
                return categories;
            foreach (CategoryItem categoryItem in result.Data.ItemList)
            {
                NaverCategory category = new NaverCategory
                {
                    Id = categoryItem.Id,
                    Name = categoryItem.Name,
                    Level = 1
                };
                FillSubCategories(category, categoryItem.ItemList);
                categories.Add(category);
            }
            return categories;
        }

        private static void FillSubCategories(NaverCategory category, List<CategoryItem> categoryItems)
        {
            foreach (CategoryItem categoryItem in categoryItems)
            {
                NaverCategory subCategory = new NaverCategory
                {
                    Id = categoryItem.Id,
                    Name = categoryItem.Name,
                    Level = category.Level + 1
                };
                if (categoryItem.ItemList is not null && categoryItem.ItemList.Count > 0)
                    FillSubCategories(subCategory, categoryItem.ItemList);
                category.SubCategories.Add(subCategory);
            }
        }

        public Task<List<Category>> GetCategories(Category category)
        {
            if (category is CoupangCategory)
                throw new InvalidOperationException("category is not naver category");
            return GetCategories((NaverCategory)category);
        }

        private Task<List<Category>> GetCategories(NaverCategory category)
        {
            return Task.FromResult(category.SubCategories);
        }

        private sealed class CategoryQueryResult
        {
            [JsonPropertyName("hamburger")]
            public CategoryQueryResultData? Data { get; set; }
        }

        private sealed class CategoryQueryResultData
        {
            [JsonPropertyName("categories")]
            public List<CategoryItem> ItemList { get; set; } = new List<CategoryItem>();
        }

        private sealed class CategoryItem
        {
            [JsonPropertyName("catId")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("catNm")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("categories")]
            public List<CategoryItem> ItemList { get; set; } = new List<CategoryItem>();
        }
    }
}
