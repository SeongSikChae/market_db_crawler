namespace MarketCrawlerLib.Crawler
{
    public interface ICrawler
    {
        Task<List<Category>> Categories(string? parameter = null);

        Task<List<Category>> Categories(Category category);
    }

    public sealed class Category
    {
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool IsSubCategory { get; set; }

        public string? Link { get; set; }

        public override string ToString()
        {
            return $"{Id}/{Name}/{IsSubCategory}/{Link}";
        }
    }
}
