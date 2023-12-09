using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCCRUD.Models.Authorizationmodels;
using MVCCRUD.Models.Domain;
using System.Security.Claims;

namespace MVCCRUD.Models.Service
{
    public class UserAuthorizationService : IUserAuthorizationService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserAuthorizationService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Status> LoginAsync(LoginModel Model)
        {
            var status = new Status();
            var user = await _userManager.FindByNameAsync(Model.Username);
            if (user == null)
            {
                status.StatusCode = 0;
                status.Message = "invalid username";
                return status;
            }
            if (!await _userManager.CheckPasswordAsync(user, Model.Password))
            {
                status.StatusCode = 0;
                status.Message = "invalid Password";
                return status;
            }
            var signinResult = await _signInManager.PasswordSignInAsync(user, Model.Password, false, true);
            if (signinResult.Succeeded)
            {
                var userroles = await _userManager.GetRolesAsync(user);
                var authclaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,user.UserName)
                };
                foreach (var role in userroles)
                {
                    authclaims.Add(new Claim(ClaimTypes.Role, role));
                }
                status.StatusCode = 1;
                status.Message = "logged successfully";
                return status;
            }
            else if (signinResult.IsLockedOut)
            {
                status.StatusCode = 0;
                status.Message = "User locked out";
                return status;
            }
            else
            {
                status.StatusCode = 0;
                status.Message = "Error in loggin";
                return status;
            }


        }

        public async Task LogoutAsync()
        {
            _signInManager.SignOutAsync();
        }

        public async Task<Status> RegistrationAsync(Registration model)
        {
            var status = new Status();
            var userexists = await _userManager.FindByNameAsync(model.Username);
            if (userexists != null)
            {
                status.StatusCode = 0;
                status.Message = "User already exists";
                return status;
            }
            ApplicationUser user = new ApplicationUser()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = model.Name,
                Email = model.Email,
                UserName = model.Username,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                status.StatusCode = 0;
                status.Message = "user creation is failed";
                return status;
            }
            if (!await _roleManager.RoleExistsAsync(model.Role))
            {
                await _roleManager.CreateAsync(new IdentityRole(model.Role));
            }
            if (await _roleManager.RoleExistsAsync(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }
            status.StatusCode = 1;
            status.Message = "User  successfully has been  registered";
            return status;

        }
    }
}
