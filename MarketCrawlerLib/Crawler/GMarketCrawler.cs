using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public sealed class GMarketCategory : Category
    {
        public List<Category> SubCategories { get; set; } = new List<Category>();
    }

    public sealed class GMarketCrawler : ICrawler
    {
        private const string UserAgentHeaderName = "User-Agent";
        private const string UserAgentHeaderValue = "Mozilla/5.0 (Linux; Android 9; SM-G973N Build/PI; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/121.0.6167.180 Mobile Safari/537.36 MobileApp/1.1 (Android; 10.5.7; com.ebay.kr.gmarket; SM-G973N)";

        public async Task<List<Category>> GetCategories()
        {
            List<Category> categories = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://elsa-fe.gmarket.co.kr");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            client.DefaultRequestHeaders.Add("Referer", "https://m.gmarket.co.kr");
            string? json = await client.GetStringAsync("api/gnb-search");
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            CategoryQueryResult? result = JsonSerializer.Deserialize<CategoryQueryResult>(json);
            if (result is null)
                return categories;
            if (result.Data is null)
                return categories;
            foreach (CategoryInfo categoryInfo in result.Data.CategoryInfoList)
            {
                GMarketCategory category = new GMarketCategory
                {
                    Id = categoryInfo.Seq.ToString(),
                    Name = categoryInfo.Name,
                    Level = 1
                };
                FillSubCategories(category, categoryInfo.TabList);
                categories.Add(category);
            }
            return categories;
        }

        private static void FillSubCategories(GMarketCategory category, List<CategoryTab> categoryTabs)
        {
            foreach (CategoryTab tab in categoryTabs)
            {
                if (string.IsNullOrWhiteSpace(tab.CategoryCode))
                    continue;

                GMarketCategory subCategory = new GMarketCategory
                {
                    Id = tab.CategoryCode,
                    Name = tab.Name,
                    Level = category.Level + 1
                };

                if (tab.Elements is not null && tab.Elements.Count > 0)
                    FillSubCategories(subCategory, tab.Elements);
                category.SubCategories.Add(subCategory);
            }
        }

        public Task<List<Category>> GetCategories(Category category)
        {
            if (category is GMarketCategory)
                throw new InvalidOperationException("category is not gmarket category");
            return GetCategories((GMarketCategory)category);
        }

        private Task<List<Category>> GetCategories(GMarketCategory category)
        {
            return Task.FromResult(category.SubCategories);
        }

        private sealed class CategoryQueryResult
        {
            [JsonPropertyName("data")]
            public CategoryQueryResultData? Data { get; set; }
        }

        private sealed class CategoryQueryResultData
        {
            [JsonPropertyName("cppCategoryInfos")]
            public List<CategoryInfo> CategoryInfoList { get; set; } = new List<CategoryInfo>();
        }

        private sealed class CategoryInfo
        {
            [JsonPropertyName("seq")]
            public int Seq { get; set; }
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("categoryTabs")]
            public List<CategoryTab> TabList { get; set; } = new List<CategoryTab>();
        }

        private sealed class CategoryTab
        {
            [JsonPropertyName("categoryCode")]
            public string CategoryCode { get; set; } = string.Empty;
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("categoryElements")]
            public List<CategoryTab> Elements { get; set; } = new List<CategoryTab>();
        }
    }
}
