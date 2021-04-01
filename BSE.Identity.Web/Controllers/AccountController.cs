using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BSE.Identity.Web.Models;
using BSE.Identity.Web.Models.EntityFramework;

namespace BSE.Identity.Web.Controllers
{
	[Authorize]
	public class AccountController : Controller
	{
		#region Properties
		public UserManager<ApplicationUser> UserManager
		{
			get;
			private set;
		}
		public RoleManager<IdentityRole> RoleManager
		{
			get;
			private set;
		}
		#endregion

        #region MethodsPublic

        public AccountController()
			: this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())), new RoleManager<IdentityRole>(
				new RoleStore<IdentityRole>(new ApplicationDbContext())))
		{
		}
		public AccountController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.UserManager = userManager;
			this.UserManager.UserValidator = new UserValidator<ApplicationUser>(this.UserManager)
			{
				AllowOnlyAlphanumericUserNames = false
			};
			this.RoleManager = roleManager;
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var user = await UserManager.FindAsync(model.UserName, model.Password);
				if (user != null)
				{
					await SignInAsync(user, model.RememberMe);
					return RedirectToLocal(returnUrl);
				}
				else
				{
					ModelState.AddModelError("", "Invalid username or password.");
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Register
		[Authorize(Roles = "administrator")]
		public ActionResult Register()
		{
			return View();
		}

		//
		// POST: /Account/Register
		[HttpPost]
		[Authorize(Roles = "administrator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new ApplicationUser()
				{
					UserName = model.UserName,
					FirstName = model.FirstName,
					LastName = model.LastName,
				};
				var result = await UserManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Account");
				}
				else
				{
					AddErrors(result);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		//
		// GET: /Account/Manage
		[Authorize(Roles = "administrator")]
		public ActionResult Manage(ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ManageMessageId.Error ? "An error has occurred."
				: "";
			ViewBag.HasLocalPassword = HasPassword();
			ViewBag.ReturnUrl = Url.Action("Manage");
			return View();
		}

		//
		// POST: /Account/Manage
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "administrator")]
		public async Task<ActionResult> Manage(ManageUserViewModel model)
		{
			bool hasPassword = HasPassword();
			ViewBag.HasLocalPassword = hasPassword;
			ViewBag.ReturnUrl = Url.Action("Manage");
			if (hasPassword)
			{
				if (ModelState.IsValid)
				{
					IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new
						{
							Message = ManageMessageId.ChangePasswordSuccess
						});
					}
					else
					{
						AddErrors(result);
					}
				}
			}
			else
			{
				// User does not have a password so remove any validation errors caused by a missing OldPassword field
				ModelState state = ModelState["OldPassword"];
				if (state != null)
				{
					state.Errors.Clear();
				}

				if (ModelState.IsValid)
				{
					IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Manage", new
						{
							Message = ManageMessageId.SetPasswordSuccess
						});
					}
					else
					{
						AddErrors(result);
					}
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		// GET: /Account/Manage
		[Authorize(Roles = "administrator")]
		public ActionResult ManagePassword(string id, ManageMessageId? message)
		{
			ViewBag.StatusMessage =
				message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
				: message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
				: message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
				: message == ManageMessageId.Error ? "An error has occurred."
				: "";
			var Db = new ApplicationDbContext();
			var user = Db.Users.First(u => u.Id == id);
			var model = new ManagePasswordViewModel(user);
			//ViewBag.HasLocalPassword = false;
			//ViewBag.ReturnUrl = Url.Action("ManagePassword");
			return View(model);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "administrator")]
		public async Task<ActionResult> ManagePassword(ManagePasswordViewModel model)
		{
			ViewBag.ReturnUrl = Url.Action("ManagePassword");
			if (ModelState.IsValid)
			{
				var Db = new ApplicationDbContext();
				var user = Db.Users.First(u => u.Id == model.Id);

				IdentityResult result = await UserManager.RemovePasswordAsync(user.Id)
					.ContinueWith<IdentityResult>(resultTask =>
					{
						return UserManager.AddPasswordAsync(user.Id, model.NewPassword).Result;
					});

				if (result.Succeeded)
				{
					return RedirectToAction("Index");
				}
				else
				{
					AddErrors(result);
				}
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}
		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			AuthenticationManager.SignOut();
			return RedirectToAction("Index", "Home");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && UserManager != null)
			{
				UserManager.Dispose();
				UserManager = null;
			}
			base.Dispose(disposing);
		}

		[Authorize(Roles = "administrator")]
		public ActionResult Index()
		{
			var Db = new ApplicationDbContext();
			var users = Db.Users;
			var model = new List<EditUserViewModel>();
			foreach (var user in users)
			{
				var u = new EditUserViewModel(user);
				model.Add(u);
			}
			return View(model);
		}

		[Authorize(Roles = "administrator")]
		public ActionResult Edit(string id, ManageMessageId? Message = null)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.First(u => u.Id == id);
			var model = new EditUserViewModel(user);
			ViewBag.MessageId = Message;
			return View(model);
		}


		[HttpPost]
		[Authorize(Roles = "administrator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(EditUserViewModel model)
		{
			if (ModelState.IsValid)
			{
				var Db = new ApplicationDbContext();
				var user = Db.Users.First(u => u.Id == model.Id);
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
				await Db.SaveChangesAsync();
				return RedirectToAction("Index");
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}


		[Authorize(Roles = "administrator")]
		public ActionResult Delete(string id = null)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.First(u => u.Id == id);
			var model = new EditUserViewModel(user);
			if (user == null)
			{
				return HttpNotFound();
			}
			return View(model);
		}


		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "administrator")]
		public ActionResult DeleteConfirmed(string id)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.First(u => u.Id == id);
			Db.Users.Remove(user);
			Db.SaveChanges();
			return RedirectToAction("Index");
		}
		// GET: /Account/CreateRole
		[Authorize(Roles = "administrator")]
		public ActionResult CreateRole()
		{
			return View();
		}

		// POST: /Account/Register
		[HttpPost]
		[Authorize(Roles = "administrator")]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> CreateRole(CreateRoleViewModel model)
		{
			if (ModelState.IsValid)
			{
				//UserManager.
				var result = await this.RoleManager.CreateAsync(new IdentityRole(model.RoleName));
				if (result.Succeeded)
				{
					return RedirectToAction("Index", "Account");
				}
			}
			// If we got this far, something failed, redisplay form
			return View(model);
		}


		[Authorize(Roles = "administrator")]
		public ActionResult UserRoles(string id)
		{
			var Db = new ApplicationDbContext();
			var user = Db.Users.First(u => u.Id == id);
			var model = new SelectUserRolesViewModel(user);
			return View(model);
		}

		[HttpPost]
		[Authorize(Roles = "administrator")]
		[ValidateAntiForgeryToken]
		public ActionResult UserRoles(SelectUserRolesViewModel model)
		{
			if (ModelState.IsValid)
			{
				var dbContext = new ApplicationDbContext();
				var user = dbContext.Users.First(u => u.Id == model.Id);
				if (user != null)
				{
					foreach (var role in user.Roles)
					{
						this.UserManager.RemoveFromRole(user.Id, role.Role.Name);
					}
					foreach (var role in model.Roles)
					{
						if (role.Selected)
						{
							this.UserManager.AddToRole(user.Id, role.RoleName);
						}
					}
				}
				return RedirectToAction("index");
			}
			return View();
		}

        #endregion

        #region Helpers

        private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private async Task SignInAsync(ApplicationUser user, bool isPersistent)
		{
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
			var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
			AuthenticationManager.SignIn(new AuthenticationProperties()
			{
				IsPersistent = isPersistent
			}, identity);
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private bool HasPassword()
		{
			var user = UserManager.FindById(User.Identity.GetUserId());
			if (user != null)
			{
				return user.PasswordHash != null;
			}
			return false;
		}

		public enum ManageMessageId
		{
			ChangePasswordSuccess,
			SetPasswordSuccess,
			RemoveLoginSuccess,
			Error
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		#endregion
	}
}