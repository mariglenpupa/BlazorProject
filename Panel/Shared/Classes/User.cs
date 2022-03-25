using Panel.Shared.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Panel.Shared.Classes
{
    public class User : IUser
    {
        public int Id { get; set; }
        [MaxLength(40)]
        public string UUID { get; set; } = "";
        [Required]
        [MaxLength(600)]
        public string UserName { get; set; } = "";
        [Required]
        [MaxLength(600)]
        public string Password { get; set; } = "";
        [MaxLength(120)]
        public string Contact { get; set; } = "";

        public Role Role { get; set; }
        public List<Key> ActiveKeys { get; set; } = new List<Key>();

        // Handle Devices & IPs
        public string MachineList { get; set; } = "";
        public string IPList { get; set; } = "";
        public DateTime Joined { get; set; } // For analytics & stuff...
    }
    public enum Role
    {
        Visitor,
        User,
        Mod,
        Admin
    }
}
