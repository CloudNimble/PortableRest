using System;

namespace PortableRest.Tests.OwinSelfHostServer
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string Genre { get; set; }
        public DateTime PublishDate { get; set; }
        public string Description { get; set; }
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}