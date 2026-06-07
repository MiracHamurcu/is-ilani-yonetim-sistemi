using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace isilani.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public RegisterModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
            public string Password { get; set; }

            [Required, DataType(DataType.Password), Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
            var res = await _userManager.CreateAsync(user, Input.Password);
            if (res.Succeeded)
            {
                if (!await _userManager.IsInRoleAsync(user, "User"))
                {
                    var roleMgr = HttpContext.RequestServices.GetRequiredService<RoleManager<IdentityRole>>();
                    if (!await roleMgr.RoleExistsAsync("User")) await roleMgr.CreateAsync(new IdentityRole("User"));
                    await _userManager.AddToRoleAsync(user, "User");
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToPage("/Index");
            }

            foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }
    }
}
