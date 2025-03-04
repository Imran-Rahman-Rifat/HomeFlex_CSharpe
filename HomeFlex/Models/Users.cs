using Microsoft.AspNetCore.Identity;

namespace HomeFlex.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string AccountType { get; set; }
    }
}
