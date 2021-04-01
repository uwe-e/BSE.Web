using BSE.Identity.Web.Models.EntityFramework;
using System.Collections.Generic;

namespace BSE.Identity.Web.Models
{
    public class SelectUserRolesViewModel
	{
		public SelectUserRolesViewModel()
		{
			this.Roles = new List<SelectRoleEditorViewModel>();
		}


		// Enable initialization with an instance of ApplicationUser:
		public SelectUserRolesViewModel(ApplicationUser user)
			: this()
		{
			this.Id = user.Id;
			this.UserName = user.UserName;

			var Db = new ApplicationDbContext();

			// Add all available roles to the list of EditorViewModels:
			var allRoles = Db.Roles;
			foreach (var role in allRoles)
			{
				// An EditorViewModel will be used by Editor Template:
				var rvm = new SelectRoleEditorViewModel(role);
				this.Roles.Add(rvm);
			}

			// Set the Selected property to true for those roles for 
			// which the current user is a member:
			foreach (var userRole in user.Roles)
			{
				var checkUserRole =
					this.Roles.Find(r => r.RoleName == userRole.Role.Name);
				checkUserRole.Selected = true;
			}
		}

		public string Id
		{
			get;
			set;
		}

		public string UserName
		{
			get;
			set;
		}

		public List<SelectRoleEditorViewModel> Roles
		{
			get;
			set;
		}
	}
}