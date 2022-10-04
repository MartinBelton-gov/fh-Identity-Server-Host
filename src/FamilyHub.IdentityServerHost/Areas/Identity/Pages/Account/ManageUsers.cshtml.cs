using FamilyHub.IdentityServerHost.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class ManageUsersModel : PageModel
{
    //private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    public string ReturnUrl { get; set; } = default!;

    public ManageUsersModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;       
    }

    public async Task OnGet()
    {
        ReturnUrl ??= Url.Content("~/Identity/Account/ManageUsers");

        var users = _userManager.Users.OrderBy(x => x.UserName).ToList();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            Users.Add(new ApplicationUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = string.Join(", ", roles)
            });
        }
        
    }
}
