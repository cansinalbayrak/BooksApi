namespace BooksApi.Models
{
    public class BookFilterModel
    {
        public string? Category { get; set; }
        public string? Color { get; set; }
        public string? Author { get; set; }
        public int? MaxP { get; set; }
        public int? MinP { get; set; }
        
    }
}
