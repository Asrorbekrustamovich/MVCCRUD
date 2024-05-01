using System.ComponentModel.DataAnnotations;

namespace MVCCRUD.Models.Domain
{
    public class Registration
    {
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }
        [Required]
        public string Role { get; set; } = "user";
    }
}
