using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Online_Shopping_Website.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebGrease.Activities;

namespace Online_Shopping_Website.Db
{
    public class MyDBInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            // Initialize default identity roles
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            // RoleTypes is a class containing constant string values for different roles
            List<IdentityRole> identityRoles = new List<IdentityRole>();
            identityRoles.Add(new IdentityRole() { Name = "Admin" });
            identityRoles.Add(new IdentityRole() { Name = "Guest" });

            var store = new UserStore<ApplicationUser>(context);

            foreach (IdentityRole role in identityRoles)
            {
                roleManager.Create(role);
            }

            // Initialize default user
            var manager = new UserManager<ApplicationUser>(store);
            ApplicationUser admin = new ApplicationUser();
            admin.Email = "admin@gmail.com";
            admin.UserName = "administrator";

            manager.Create(admin, "1Admin!");
            manager.AddToRole(admin.Id, "Admin");

            // Add code to initialize context tables

            base.Seed(context);
        }
    }
}