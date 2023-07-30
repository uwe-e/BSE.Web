using BSE.Identity.Web.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace BSE.Identity.Web.Models.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		static ApplicationDbContext()
		{
			Database.SetInitializer(new MySqlInitializer());
		}

		public ApplicationDbContext()
			: base("DefaultConnection")
		{
		}
	}
}