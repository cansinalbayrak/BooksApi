namespace BooksApi.Models
{
    public class AddBookModel
    {
        public int Page { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Author { get; set; }
        public string Color { get; set; }
    }
}
