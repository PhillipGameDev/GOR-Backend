﻿using System;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using GameOfRevenge.WebAdmin.Configurations.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebAdmin.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] public string LoginUsername { get; set; }
        [BindProperty] public string LoginPassword { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var users = InMemoryConfig.GetUsers();
            var user = users.Find(x => (x.Username == LoginUsername) && (x.Password == LoginPassword));
            if (user != null)
            {
                var claimsIdentity = new ClaimsIdentity(user.Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                            new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToPage("/Index");
            }
            else
            {
                return Page();
            }
        }
    }
}