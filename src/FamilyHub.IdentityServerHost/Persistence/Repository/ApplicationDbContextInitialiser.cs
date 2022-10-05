﻿using FamilyHub.IdentityServerHost.Models.Entities;
using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Security.Claims;
using System.Threading;

namespace FamilyHub.IdentityServerHost.Persistence.Repository;

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IApiService _apiService;

    private List<OpenReferralOrganisationDto> _openReferralOrganisationDtos = default!;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IApiService apiService)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _apiService = apiService;
    }

    public async Task InitialiseAsync(IConfiguration configuration)
    {
        try
        {
            if (_context.Database.IsSqlServer() || _context.Database.IsNpgsql())
            {
                //if (configuration.GetValue<bool>("RecreateDbOnStartup"))
                //{
                //    _context.Database.EnsureDeleted();
                //    _context.Database.EnsureCreated();
                //}
                //else
                await _context.Database.MigrateAsync();
            }
            //else
            //{
            //    _context.Database.EnsureDeleted();
            //}
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        await EnsureRoles();
        await EnsureUsers();
    }

    private async Task EnsureRoles()
    {
        if (!_roleManager.Roles.Any())
        {
            _logger.LogDebug("Roles being populated");
            string[] roles = new string[] { "DfEAdmin", "LAAdmin", "VCSAdmin", "Pro" };
            foreach (var role in roles)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }
        }
        else
        {
            _logger.LogDebug("Roles already populated");
        }

    }

    private async Task EnsureUsers()
    {
        var user = _userManager.FindByNameAsync("DfEAdmin").Result;
        if (user != null)
        {
            _logger.LogDebug("Users already populated");
            return;
        }

        _openReferralOrganisationDtos = await _apiService.GetListOpenReferralOrganisations();

        string[] LAAdmins = new string[] { "BtlLAAdmin", "LanLAAdmin", "LbrLAAdmin", "SalLAAdmin", "SufLAAdmin", "TowLAAdmin" };
        string[] SvcAdmins = new string[] { "BtlVCSAdmin", "LanVCSAdmin", "LbrVCSAdmin", "SalVCSAdmin", "SufVCSAdmin", "TowVCSAdmin" };
        string[] Pro = new string[] { "BtlPro", "LanPro", "LbrPro", "SalPro", "SufPro", "TowPro" };
        string[] Websites = new string[] { "https://www.bristol.gov.uk/", "https://www.lancashire.gov.uk/", "https://www.redbridge.gov.uk/", "https://www.salford.gov.uk/", "https://www.suffolk.gov.uk/", "https://www.towerhamlets.gov.uk/Home.aspx" };

        await AddUser(_userManager, "DfEAdmin", "Pass123$", "DfEAdmin", "www.warmhandover.gov.uk");
        for (int i = 0; i < LAAdmins.Length; i++)
        {
            await AddUser(_userManager, LAAdmins[i], "Pass123$", "LAAdmin", Websites[i]);
        }
        for (int i = 0; i < SvcAdmins.Length; i++)
        {
            await AddUser(_userManager, SvcAdmins[i], "Pass123$", "VCSAdmin", Websites[i]);
        }
        for (int i = 0; i < Pro.Length; i++)
        {
            await AddUser(_userManager, Pro[i], "Pass123$", "Pro", Websites[i]);
        }
    }

    private async Task AddUser(UserManager<IdentityUser> userMgr, string person, string password, string role, string website)
    {
        var user = userMgr.FindByNameAsync(person).Result;
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = person,
                Email = $"{person}@email.com",
                EmailConfirmed = true,
            };
            var result = userMgr.CreateAsync(user, password).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(user, new Claim[]
            {
        new Claim(JwtClaimTypes.Name, person),
        new Claim(JwtClaimTypes.GivenName, person),
        new Claim(JwtClaimTypes.Role, role),
        //new Claim(JwtClaimTypes.FamilyName, "Smith"),
        //new Claim(JwtClaimTypes.WebSite, "http://warmhandover.gov.uk"),
            }).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = await userMgr.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            if (_openReferralOrganisationDtos != null && _openReferralOrganisationDtos.Any())
            {
                var currentuser = userMgr.FindByNameAsync(person).Result;
                if (currentuser != null)
                {
                    OpenReferralOrganisationDto? organisation = null;
                    if (person.StartsWith("Btl"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("Bristol"));
                    }
                    if (person.StartsWith("Lan"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("Lancashire"));
                    }
                    if (person.StartsWith("Lbr"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("London Borough of Redbridge"));
                    }
                    if (person.StartsWith("Sal"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("Salford"));
                    }
                    if (person.StartsWith("Suf"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("Suffolk"));
                    }
                    if (person.StartsWith("Tow"))
                    {
                        organisation = _openReferralOrganisationDtos.FirstOrDefault(x => x.Name != null && x.Name.StartsWith("Suffolk"));
                    }

                    if (organisation != null)
                    {
                        _context.UserOrganisations.Add(new UserOrganisation(Guid.NewGuid().ToString(), currentuser.Id, organisation.Id));
                        await _context.SaveChangesAsync();
                    }
                }
            }

            _logger.LogDebug($"{person} created");
        }
        else
        {
            _logger.LogDebug($"{person} already exists");
        }
    }

    //private async Task EnsureOrganisationTypes()
    //{
    //    if (_context.OrganisationTypes.Any())
    //    {
    //        return;
    //    }

    //    List<OrganisationType> organisationTypes = new()
    //    {
    //        new OrganisationType("1", "LA", "Local Authority"),
    //        new OrganisationType("2", "VCFS", "Voluntary, Charitable, Faith Sector"),
    //        new OrganisationType("3", "Company", "Public or Private Company")
    //    };

    //    _context.OrganisationTypes.AddRange(organisationTypes);
    //    await _context.SaveChangesAsync();

    //}
}
