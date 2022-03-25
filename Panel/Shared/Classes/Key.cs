using Panel.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Panel.Shared.Classes
{
    public class Key : IKey
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductKey { get; set; } = "";
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; } = "";
        public Package Package { get; set; } // To manage how much Ips/Machines is the user allowed to use.
        [Required]
        [MaxLength(2000)] // this will also be used for token. just to be safe I set at 2k.
        public string AddedBy { get; set; } = "";
        public virtual User? User { get; set; }

        public TimeSpan LicenseLength { get; set; } = TimeSpan.FromDays(30);
        public DateTime StartDate { get; set; } = DateTime.MinValue;

        [NotMapped]
        public bool IsValid
        {
            get
            {
                if (StartDate == DateTime.MinValue)
                    return true;

                if (StartDate.AddDays(LicenseLength.Days) > DateTime.Now)
                    return true;

                return false;
            }
        }
    }
    
    public enum Package
    {
        Basic,
        Standard,
        Premium
    }
}
