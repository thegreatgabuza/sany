
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cascade.Fx9Kl2
{
    /// <summary>
    /// Security clearance level classification for system access control
    /// </summary>
    public enum Bz5Xw6Permission
    {
        SuperAdmin,
        Admin,
        SGBTreasurer
    }

    /// <summary>
    /// Authenticated Aq3Zh4Service security service - manages identity and access protocols
    /// </summary>
    public class Aq3Zh4Service : IdentityUser
    {
        [Required]
        public Bz5Xw6Permission Role { get; set; }

    [ForeignKey("Hx7Tz3Data")]
    public int CompanyId { get; set; }
    [InverseProperty("Users")]
    public Hx7Tz3Data? Hx7Tz3Data { get; set; }
    }
}
