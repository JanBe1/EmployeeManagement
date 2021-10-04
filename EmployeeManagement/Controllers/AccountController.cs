using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> Logout() 
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl) 
        {
            if (ModelState.IsValid) 
            {
               
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    //return user to an action he last did via returnUrl, eg. you tried to register a user -> you log in -> you go back to register view, not a home one
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))//prevents open redirect vulnerability
                    {
                        return Redirect(returnUrl); //i can also use localredirect
                    }
                    else 
                    {
                        return RedirectToAction("index", "home");
                    }
                    
                }
                ModelState.AddModelError(string.Empty, "Invalid username or password.");

            }
            
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email) //remote validation
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) 
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use");
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model) 
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                }; //get user from form and the model from viewmodel
                var result = await userManager.CreateAsync(user, model.Password); //create user with the password

                if (result.Succeeded)
                {
                    if(signInManager.IsSignedIn(User) && User.IsInRole("Administrator"))
                    {
                        return RedirectToAction("ListUsers", "Administration");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false); //login registered user without session cookies
                    return RedirectToAction("index", "home"); //get user back to home view
                }
                foreach (var error in result.Errors) //show all errors
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() 
        {
            return View("AccessDenied");
        }

    }
}
