﻿using EmployeeManagement.Models;
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
    //role based authorisation:
    [Authorize(Roles = "Administrator")]
    //[Authorize(Roles = "Administrator, User")] //-> administrator or user

    //Administrator AND admin required:
    //[Authorize(Roles = "Administrator")]
    //[Authorize(Roles = "User")]
    //[AllowAnonymous] -> no role is required and everybody can access it
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid) 
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded) 
                {
                    return RedirectToAction("ListRoles", "administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult ListRoles() 
        {
            var roles = roleManager.Roles;
            return View(roles);
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id) 
        {
            var role = await roleManager.FindByIdAsync(id);
            if(role == null) 
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in await userManager.GetUsersInRoleAsync(role.Name))
            {
                model.Users.Add(user.UserName);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else 
            {
                role.Name = model.RoleName;
                var result = await roleManager.UpdateAsync(role);
                if (result.Succeeded) 
                {
                    return RedirectToAction("ListRoles");
                }
                foreach(var error in result.Errors) 
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId) 
        {
            ViewBag.roleId = roleId;

            //find a user with roleId: 
            var role = await roleManager.FindByIdAsync(roleId);
            //user not found:
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();
            foreach(var user in userManager.Users) //<- userManager.Users return list of all registered users in Db
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                //another cool identity trait -> how to check if a user is in given role? 
                //userManager.IsInRoleAsync -> returns true or false if the user is in given role 
                
                //based on if user is in a given role - do IsSelected = true;
                if(await userManager.IsInRoleAsync(user, role.Name)) 
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }
                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id {roleId} cannot be found";
                return View("NotFound");
            }
            for(int i=0;i<model.Count; i++) 
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;
                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && (await userManager.IsInRoleAsync(user, role.Name))) 
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else 
                {
                    continue;
                }
                if (result.Succeeded) 
                {
                    if(i < (model.Count - 1)) 
                    {
                        continue;
                    }
                    else 
                    {
                        return RedirectToAction("EditRole", new {Id = roleId});
                    }
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        public IActionResult ListUsers() 
        {
            var users = userManager.Users;
            return View(users);
        }
    }
}