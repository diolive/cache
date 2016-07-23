using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DioLive.Cache.WebUI.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Budgets = new HashSet<Budget>();
        }

        public virtual ICollection<Budget> Budgets { get; set; }
    }
}