using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using BSE.Identity.Web.Entity;
using DataAnnotationsExtensions;

namespace BSE.Identity.Web.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser
	// class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		[Required]
        [DataType(DataType.EmailAddress)]
        [Email]
		public override string UserName
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}
		//[Required]
		//[DataType(DataType.EmailAddress)]
		//[Email]
		//public string Email
		//{
		//	get;
		//	set;
		//}
	}

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