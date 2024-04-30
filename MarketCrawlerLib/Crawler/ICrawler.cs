namespace MarketCrawlerLib.Crawler
{
    public interface ICrawler
    {
        Task<List<Category>> GetCategories();

        Task<List<Category>> GetCategories(Category category);
    }

    public sealed class Category
    {
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsSubCategory { get; set; }

        public string? Link { get; set; }

        public int Level { get; set; } = 1;

        public override string ToString()
        {
            return $"{Id}/{Name}/{IsSubCategory}/{Link}/{Level}";
        }
    }
}
