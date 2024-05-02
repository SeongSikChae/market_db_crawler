using System.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public sealed class Street11Category : Category
    {
        public bool IsSubCategory { get; set; }

        public string? Link { get; set; }
    }

    public sealed class Street11Crawler : ICrawler
    {
        public async Task<List<Category>> GetCategories()
        {
            return await GetCategories("pageId=SIDEMENU_V3", false, 1);
        }

        public async Task<List<Category>> GetCategories(Category category)
        {
            if (category is CoupangCategory)
                throw new InvalidOperationException("category is not street11 category");
            return await GetCategories((Street11Category)category);
        }

        private async Task<List<Category>> GetCategories(Street11Category category)
        {
            return await GetCategories(category.Link, category.IsSubCategory, category.Level + 1);
        }

        private const string UserAgentHeaderName = "User-Agent";
        private const string UserAgentHeaderValue = "Mozilla/5.0 (Linux; Android 9; SM-G973N Build/PI; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/121.0.6167.180 Mobile Safari/537.36 CP_ELEVENST (01; 9.7.6; playstore; 225; c29885f466388121818af4502c806fd3; 93ba8d34-bebc-42a5-82b8-196502c34a2c) CP_SESSION_ID (34b7401b-c91c-3de3-a816-1b6b4f9f146c_1714466428885; 34b7401b-c91c-3de3-a816-1b6b4f9f146c_1714466428886) SKpay/1.8.1 11pay/1.8.1 (Android 9; plugin-mode) com.elevenst/9.7.6";
        private const string BlockType = "GridList_ImgTextCard";

        private async Task<List<Category>> GetCategories(string? parameter, bool isLastCategory, int level)
        {
            if (isLastCategory) {
                HttpClient lastCategoryClient = new HttpClient();
                lastCategoryClient.BaseAddress = new Uri("http://apis.11st.co.kr");
                lastCategoryClient.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
                return await GetCategories(lastCategoryClient, $"display-api/display/category?pageId=MOCATEGORYDEFAULT&appId=01&appType=appmw&appVCA=976&deviceID={Guid.NewGuid()}&tStoreYN=N&deviceType=android&{parameter}", level);
            }
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://m.11st.co.kr");
            client.DefaultRequestHeaders.Add(UserAgentHeaderName, UserAgentHeaderValue);
            return await GetCategories(client, $"MW/CMS/PageDataAjax.tmall?{parameter}", level);
        }

        private async Task<List<Category>> GetCategories(HttpClient client, string requestUri, int level)
        {
            List<Category> categories = new List<Category>();
            string? json = await client.GetStringAsync(requestUri);
            if (string.IsNullOrWhiteSpace(json))
                return categories;
            CategoryQueryResult? result = JsonSerializer.Deserialize<CategoryQueryResult>(json);
            if (result is null)
                return categories;
            foreach (CategoryQueryResultData data in result.Datas.Where(d => "BlankCarrier".Equals(d.Type) || "CaptionCarrier".Equals(d.Type)))
            {
                foreach (CategoryQueryResultDataBlock block in data.BlockList.Where(b => b.Type.Contains(BlockType)))
                {
                    foreach (CategoryQueryResultDataBlockItem item in block.List)
                    {
                        string cate_no = string.Empty;
                        if (item.DispCtgrNo is not null)
                            cate_no = item.DispCtgrNo;
                        if (item.DispObjNo is not null)
                            cate_no = item.DispObjNo;

                        string link = string.Empty;
                        if (item.Link is not null)
                            link = item.Link;
                        if (item.LinkUrl1 is not null)
                            link = item.LinkUrl1;
                        Street11Category category = new Street11Category
                        {
                            Id = cate_no,
                            Name = item.Title1,
                            IsSubCategory = item.DispObjNo is not null,
                            Level = level
                        };
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            if (!category.IsSubCategory && link.Contains("pageId"))
                                link = link.Substring(link.IndexOf('?') + 1);
                            else
                                link = $"metaCtgrNo={cate_no}";
                        }
                        category.Link = link;
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }

        private sealed class CategoryQueryResult
        {
            [JsonPropertyName("data")]
            public List<CategoryQueryResultData> Datas { get; set; } = new List<CategoryQueryResultData>();
        }

        private sealed class CategoryQueryResultDataBlockItem
        {
            [JsonPropertyName("dispObjNo")]
            public string? DispObjNo { get; set; }

            [JsonPropertyName("dispCtgrNo")]
            public string? DispCtgrNo { get; set; }

            [JsonPropertyName("link")]
            public string? Link { get; set; }

            [JsonPropertyName("linkUrl1")]
            public string? LinkUrl1 { get; set; }

            [JsonPropertyName("title1")]
            public string Title1 { get; set; } = string.Empty;
        }

        private sealed class CategoryQueryResultDataBlock
        {
            [JsonPropertyName("type")]
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("list")]
            public List<CategoryQueryResultDataBlockItem> List { get; set; } = new List<CategoryQueryResultDataBlockItem>();
        }

        private sealed class CategoryQueryResultData
        {
            [JsonPropertyName("type")]
            public string? Type { get; set; }

            [JsonPropertyName("blockList")]
            public List<CategoryQueryResultDataBlock> BlockList { get; set; } = new List<CategoryQueryResultDataBlock>();

        }
    }
}
