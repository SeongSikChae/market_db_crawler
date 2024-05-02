using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public sealed class AuctionCrawler : ICrawler
    {
        private const string UserAgentHeaderName = "User-Agent";
        private const string AppVersion = "8.5.51";
        private const string UserAgentHeaderValue = $"MobileApp/1.0 (Android; {AppVersion}; com.ebay.kr.auction)";
        private const string AuthorizationHeaderName = "Authorization";
        private const string AuthorizationHeaderValue = "Android IQBnAG0AeQBIAEQAeQBwADMAdQBnAHEATgBsAGYAcQAyAEgALwAxAEsATwA1AEUAYQAxAHAAZgBNAGQAZgA2ACsAbgBKAE8AWQBiAFEAVQBqAHIAUgBtAEoAVAB0AEcAVgBCAFUAMwA2AG4ANABUAHAASQBYADMAaABnAHkATABrAFoAVQBhAGUASABLAHkAeQBlAGEASgBLAGUAYQBIAGUAMwBnAE8AQgBnAHkAegA5AFkAUgBVAGEAdgBoADUAdQBGADUAaABtAFoAWQBDAGYAcwBKAGsAMgA3ADcAcABhAFkANwBiAEEAegBxAHQAbgB1AG0ANQB2AGQATABkAEkAMgArADMASQA3AG8AMgBBAGEAeABzAEEARABQAFoAagBvAGIAWQA0AHgAZwA9AD0A";
        private const string AppInfoHeaderName = "App-Info";

        public async Task<List<Category>> GetCategories()
        {
            List<Category> categories = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://imapi.auction.co.kr");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            client.DefaultRequestHeaders.Add(AuthorizationHeaderName, AuthorizationHeaderValue);
            client.DefaultRequestHeaders.Add(AppInfoHeaderName, AppVersion);
            string? json = await client.GetStringAsync("api/GNB/GetCategoryGroupAndCornersListV2");
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            CategoryQueryResult? result = JsonSerializer.Deserialize<CategoryQueryResult>(json);
            if (result is null)
                return categories;
            if (result.Data is null)
                return categories;
            foreach (MobileAuctionCategoryServiceItem item in result.Data.ItemList)
            {
                categories.Add(new Category
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = 1
                });
            }
            return categories;
        }

        public async Task<List<Category>> GetCategories(Category category)
        {
            List<Category> categories = new List<Category>();
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://imapi.auction.co.kr");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            client.DefaultRequestHeaders.Add(AuthorizationHeaderName, AuthorizationHeaderValue);
            client.DefaultRequestHeaders.Add(AppInfoHeaderName, AppVersion);
            string? json = await client.GetStringAsync($"api/Category/GetCategoryList?id={category.Id}&type=normal&sort=0");
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            SubCategoryQueryResult? result = JsonSerializer.Deserialize<SubCategoryQueryResult>(json);
            if (result is null)
                return categories;
            foreach (MobileAuctionCategoryServiceItem item in result.DataList)
            {
                categories.Add(new Category
                {
                    Id = item.Id,
                    Name = item.Name,
                    Level = category.Level + 1
                });
            }
            return categories;
        }

        private sealed class CategoryQueryResult
        {
            [JsonPropertyName("Data")]
            public CategoryQueryResultData? Data { get; set; }
        }

        private sealed class CategoryQueryResultData
        {
            [JsonPropertyName("MobileAuctionCategoryService")]
            public List<MobileAuctionCategoryServiceItem> ItemList { get; set; } = new List<MobileAuctionCategoryServiceItem>();
        }

        private sealed class MobileAuctionCategoryServiceItem
        {
            [JsonPropertyName("Id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("Name")]
            public string Name { get; set; } = string.Empty;
        }

        private sealed class SubCategoryQueryResult
        {
            [JsonPropertyName("Data")]
            public List<MobileAuctionCategoryServiceItem> DataList { get; set; } = new List<MobileAuctionCategoryServiceItem>();
        }
    }
}
