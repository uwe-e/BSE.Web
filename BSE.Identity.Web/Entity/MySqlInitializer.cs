using BSE.Identity.Web.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BSE.Identity.Web.Entity
{
	public class MySqlInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
	{
		protected override void Seed(ApplicationDbContext context)
		{
			var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false
            };
			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

			string roleName = "administrator";
			string password = "123456";

			//Create Role Admin if it does not exist
			if (!roleManager.RoleExists(roleName))
			{
				var roleresult = roleManager.Create(new IdentityRole(roleName));
			}
			//Create User=Admin with password=123456
			var user = new ApplicationUser();
			user.UserName = "admin@bsetunes.com";
			var adminresult = userManager.Create(user, password);
			//Add User Admin to Role Admin
			if (adminresult.Succeeded)
			{
				var result = userManager.AddToRole(user.Id, roleName);
			}
			base.Seed(context);
		}
	}
}