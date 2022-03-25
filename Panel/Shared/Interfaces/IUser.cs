using Panel.Shared.Classes;
using System.ComponentModel.DataAnnotations;

namespace Panel.Shared.Interfaces
{
    public interface IUser
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public string UUID { get; set; }
        [Required]
        [MaxLength(600)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(600)]
        public string Password { get; set; }

        [MaxLength(120)]
        public string Contact { get; set; }
        public Role Role { get; set; }
    }
}
