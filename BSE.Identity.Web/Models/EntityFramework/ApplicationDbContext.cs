using BSE.Identity.Web.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace BSE.Identity.Web.Models.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /*
		 * property RefreshTokens is necessary because of preventing the error:
		 * 
		 * The model backing the 'ApplicationDbContext' context has changed since the database was created.
		 * Consider using Code First Migrations to update the database
		 */
        DbSet<RefreshToken> RefreshTokens { get; set; }
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