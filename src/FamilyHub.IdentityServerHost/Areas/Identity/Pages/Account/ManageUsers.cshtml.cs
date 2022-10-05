using FamilyHub.IdentityServerHost.Models;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralTaxonomys;
using FamilyHubs.SharedKernel;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyHub.IdentityServerHost.Areas.Identity.Pages.Account;

public class ManageUsersModel : PageModel
{
    
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public int PageNumber { get; set; } = 1;
    [BindProperty]
    public int PageSize { get; set; } = 10;

    public int TotalPages { get; set; } = 1;

    public PaginatedList<ApplicationUser> Users { get; set; } = new PaginatedList<ApplicationUser>();
    public string ReturnUrl { get; set; } = default!;

    public ManageUsersModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;       
    }

    //public async Task OnGet()
    //{
    //    await GetPage();
    //}

    public async Task OnGet(string pageNumber)
    {
        if (!int.TryParse(pageNumber, out var page))
        {
            page = 1;
        }

        var totalPages = _userManager.Users.Count() / PageSize;
        if (page < 1)
        {
            PageNumber = 1;    
        }
        else if (page > totalPages)
        {
            PageNumber = totalPages;
        }
        else
        {
            PageNumber = page;
        }

        await GetPage();
    }

    private async Task GetPage()
    {
        ReturnUrl ??= Url.Content("~/Identity/Account/ManageUsers");

        var users = _userManager.Users.OrderBy(x => x.UserName).ToList();
        List<ApplicationUser> applicationUsers = new();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            applicationUsers.Add(new ApplicationUser()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = string.Join(", ", roles)
            });
        }

        var pagelist = applicationUsers.Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
        Users = new PaginatedList<ApplicationUser>(pagelist, pagelist.Count, PageNumber, PageSize);
        TotalPages = applicationUsers.Count / PageSize;
    }
}
