
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public sealed class CoupangCategory : AbstractCategory
    {
        public override CategoryType CategoryType => CategoryType.Coupang;

        public List<CoupangCategory> SubCategories { get; set; } = new List<CoupangCategory>();
    }

    public sealed class CoupangCrawler : ICrawler<CoupangCategory>
    {
        private const string UserAgentHeaderName = "User-Agent";
        private const string UserAgentHeaderValue = $"Dalvik/2.1.0 (Linux; U; Android 7.1.2; SM-G975N Build/N2G48H)";
        private const string CoupangAppHeaderName = "Coupang-App";
        private const string CoupangAppHeaderValue = "COUPANG|Android|7.1.2|7.2.7|";
        private const string CoupangTargetMarketHeaderName = "X-Coupang-Target-Market";
        private const string CoupangTargetMarketHeaderValue = "KR";
        private const string CoupangOriginRegionHeaderName = "X-Coupang-Origin-Region";
        private const string CoupangOriginRegionHeaderValue = "KR";
        private const string CoupangAcceptLanguageHeaderName = "X-Coupang-Accept-Language";
        private const string CoupangAcceptLanguageHeaderValue = "ko-KR";

        public async Task<List<CoupangCategory>> GetCategories()
        {
            List<CoupangCategory> categories = new List<CoupangCategory>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://cmapi.coupang.com");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            client.DefaultRequestHeaders.Add(CoupangAppHeaderName, CoupangAppHeaderValue);
            client.DefaultRequestHeaders.Add(CoupangTargetMarketHeaderName, CoupangTargetMarketHeaderValue);
            client.DefaultRequestHeaders.Add(CoupangOriginRegionHeaderName, CoupangOriginRegionHeaderValue);
            client.DefaultRequestHeaders.Add(CoupangAcceptLanguageHeaderName, CoupangAcceptLanguageHeaderValue);
            string? json = await client.GetStringAsync("v3/categories/home");
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            CategoryQueryResult? result = JsonSerializer.Deserialize<CategoryQueryResult>(json);
            if (result is null)
                return categories;
            if (result.RData is null)
                return categories;
            foreach (CategoryItem item in result.RData.CategoryList)
            {
                if (!int.TryParse(item.Id, out _))
                    continue;
                if ("서비스".Equals(item.Name))
                    continue;
                CoupangCategory category = new CoupangCategory
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = 1
                };

                foreach (CategoryItem subItem in item.Children)
                {
                    if (!int.TryParse(subItem.Id, out _))
                        continue;
                    if ("0".Equals(subItem.Id))
                        continue;

                    category.SubCategories.Add(new CoupangCategory
                    {
                        Id = subItem.Id,
                        Name = subItem.Name,
                        Level = category.Level + 1
                    });
                }

                categories.Add(category);
            }
            return categories;
        }

        public async Task<List<CoupangCategory>> GetCategories(CoupangCategory category)
        {
            return await Task.FromResult(category.SubCategories);
        }

        private sealed class CategoryQueryResult
        {
            [JsonPropertyName("rData")]
            public CategoryQueryResultData? RData { get; set; }
        }

        private sealed class CategoryQueryResultData
        {
            [JsonPropertyName("categoryList")]
            public List<CategoryItem> CategoryList { get; set; } = new List<CategoryItem>();
        }

        private sealed class CategoryItem
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;
            [JsonPropertyName("children")]
            public List<CategoryItem> Children { get; set; } = new List<CategoryItem>();
        }
    }
}
