using System.ComponentModel.DataAnnotations;

namespace Panel.Shared.Classes
{
    public class Product
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = "";
        [MaxLength(15)]
        public string Version { get; set; } = "";
    }
}
