using System.ComponentModel.DataAnnotations;

namespace Quotes.Repository
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }


        [Required]
        public string Name { get; set; }
    }
}
