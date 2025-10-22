using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace MauiBlazorWeb.Web.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string? First { get; set; }
        public string? Last { get; set; }
        
        // Navigation property to shows this user is judging
        public virtual ICollection<Show> ShowsJudging { get; set; } = new List<Show>();
        
        // Navigation property to horses this user owns
        public virtual ICollection<UserModelObject> Horses { get; set; } = new List<UserModelObject>();
    }
}