using System.Text.Json.Serialization;

namespace MarketCrawlerLib.Crawler
{
    public interface ICrawler
    {
        Task<List<Category>> GetCategories();

        Task<List<Category>> GetCategories(Category category);
    }

    public class Category
    {
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        public override string ToString()
        {
            return $"{Id}/{Name}/{Level}";
        }
    }
}
