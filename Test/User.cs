using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test
{
    [Table("User")]
    public class User : IdentityUser
    {
        [PersonalData]
        public string? FullName { get; set; }
        [PersonalData]
        public DateTime? DateOfBirth { get; set; }
    }
}
