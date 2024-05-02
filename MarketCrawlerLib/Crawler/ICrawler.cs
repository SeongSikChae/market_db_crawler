using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public interface ICrawler<TCategory> where TCategory : AbstractCategory
    {
        Task<List<TCategory>> GetCategories();

        Task<List<TCategory>> GetCategories(TCategory category);
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum CategoryType
    {
        Auction, Coupang, GMarket, Naver, Street11
    }

    public abstract class AbstractCategory
    {
        public abstract CategoryType CategoryType { get; }

        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        public override string ToString()
        {
            return $"{Id}/{Name}/{Level}";
        }
    }
}
