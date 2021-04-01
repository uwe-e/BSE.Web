﻿using BSE.Identity.Web.Models.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BSE.Identity.Web.Models
{
    public class ManagePasswordViewModel
	{
		public ManagePasswordViewModel()
		{
		}

		public ManagePasswordViewModel(ApplicationUser user) : this()
		{
			this.Id = user.Id;
		}

		public string Id
		{
			get;
			set;
		}

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword
		{
			get;
			set;
		}

		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword
		{
			get;
			set;
		}
	}
}