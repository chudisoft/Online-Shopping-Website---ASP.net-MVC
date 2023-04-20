using System.ComponentModel.DataAnnotations;

namespace Online_Shopping_Website.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required, MaxLength(150), MinLength(2)]
        public string Name { get; set; }
    }
}