using Panel.Shared.Classes;
using System.ComponentModel.DataAnnotations;

namespace Panel.Shared.Interfaces
{
    public interface IKey
    {
        [Required]
        [MaxLength(100)]
        public string ProductKey { get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        public Package Package { get; set; }
        [Required]
        [MaxLength(2000)]
        public string AddedBy { get; set; }

        public TimeSpan LicenseLength { get; set; }
    }
}
