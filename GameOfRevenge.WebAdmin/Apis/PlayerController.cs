using System;
using System.Collections.Generic;
using GameOfRevenge.Common.Interface.UserData;
using GameOfRevenge.WebAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameOfRevenge.WebAdmin.Apis
{
    public class PlayerController : Controller
    {
        /*        public IActionResult Index()
                {
                    List<User> users = GetUsersFromDatabase();

                    return View(users);
                }*/

        private readonly IBaseUserManager userManager;

        private readonly ILogger<PlayerController> _logger;

        public PlayerController()
        {
        }

        public PlayerController(IBaseUserManager userManager, ILogger<PlayerController> logger)
        {
            this.userManager = userManager;
            _logger = logger;
        }
    }
}
