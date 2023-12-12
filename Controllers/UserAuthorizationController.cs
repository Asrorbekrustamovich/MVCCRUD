using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCCRUD.Models.Authorizationmodels;
using MVCCRUD.Models.Domain;
using MVCCRUD.Models.Service;
using System.Security.Cryptography.X509Certificates;

namespace MVCCRUD.Controllers
{
    public class UserAuthorizationController : Controller
    { private readonly  IUserAuthorizationService _userAuthorizationService;

       
        public UserAuthorizationController(IUserAuthorizationService userAuthorizationService)
        {
            _userAuthorizationService = userAuthorizationService;
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Registration(Registration model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            model.Role = "user";
            var result=await _userAuthorizationService.RegistrationAsync(model);
            TempData["msg"] = result.Message;
            return RedirectToAction(nameof(Login));
            
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            var result=await _userAuthorizationService.LoginAsync(model);
            if(result.StatusCode==1)
            {
                return RedirectToAction("Display", "Dashboard");
            }
            else
            {
                TempData["msg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }
           
        }
        [Authorize]
        public async Task<IActionResult> logout()
        {
            await _userAuthorizationService.LogoutAsync();
            return RedirectToAction("Login", "UserAuthorization");
        }
        //public async Task<IActionResult> reg()
        //{
        //    var model = new Registration()
        //    {
        //        Username = "admin",
        //        Name = "Max Wells",
        //        Email = "Max@gmail.com",
        //        Password = "Admin@12345"
        //    };
        //    model.Role = "admin";
        //    var result = await _userAuthorizationService.RegistrationAsync(model);
        //    return Ok(result);
        //}
    }
}
