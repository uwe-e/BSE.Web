using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BSE.Identity.Web.Models
{
	// Used to display a single role with a checkbox, within a list structure:
	public class SelectRoleEditorViewModel
	{
		public SelectRoleEditorViewModel()
		{
		}
		public SelectRoleEditorViewModel(IdentityRole role)
		{
			this.RoleName = role.Name;
		}

		public bool Selected
		{
			get;
			set;
		}

		[Required]
		public string RoleName
		{
			get;
			set;
		}
	}
}