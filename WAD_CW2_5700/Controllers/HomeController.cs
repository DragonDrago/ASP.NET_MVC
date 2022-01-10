using DAL.Models;
using DAL.Models.Authentication;
using DAL.Repositories;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Security;

namespace WAD_CW2_5700.Controllers
{
    public class HomeController : Controller
    {
        UserRepository _userRepo = new UserRepository();
        private static Logger logger = LogManager.GetLogger("WebSite");

        // GET: Account
        [Authorize]
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");
            return View();

        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Registration()
        {
            return View(new RegistrationVM());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Registration(RegistrationVM register)
        {
            if (register.Password != register.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Does not match with the password");

            if (ModelState.IsValid)
            {
                if (_userRepo.GetAll().Any(u => u.Email == register.Email))
                {
                    ModelState.AddModelError("Email", "User with such email already registered");
                }
                else
                {
                    var user = new User
                    {
                        Email = register.Email,
                        Password = register.Password,
                        FullName = register.Firstname + " " + register.Lastname
                    };

                    _userRepo.Create(user);

                    logger.Info(register.Email + " have registered successefully");

                    return RedirectToAction("Login");
                }

            }

            return View(register);


        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {

            return View(new LoginVM());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginVM login)
        {
            if (ModelState.IsValid && ValidateCaptcha())
            {

                if (!_userRepo.GetAll().Any(u => u.Email == login.Email && u.Password == login.Password))
                {
                    ModelState.AddModelError("Email", "Wrong credentials");
                }
                else
                {
                    FormsAuthentication.SetAuthCookie(login.Email, false);

                    logger.Info(login.Email + " have logged in successfully");

                    return RedirectToAction("Index","Products");
                }

            }


            return View(login);
        }

        [HttpGet]
        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            logger.Info(User.Identity.Name + " have Logged out successfully");

            return RedirectToAction("Login");
        }

        private class CaptchaResponse
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorCodes { get; set; }
        }

        private bool ValidateCaptcha()
        {
            var client = new WebClient();
            var reply =
                client.DownloadString(
                    string.Format("https://www.google.com/recaptcha/api/siteverify?secret=6LeGfNoUAAAAAAFlO-qQv3gavnbyZ9_XJ509ToU_&response={0}", Request["g-recaptcha-response"]));
            var captchaResponse = JsonConvert.DeserializeObject<CaptchaResponse>(reply);
            //when response is false check for the error message
            if (!captchaResponse.Success)
            {
                if (captchaResponse.ErrorCodes.Count <= 0) return false;
                var error = captchaResponse.ErrorCodes[0].ToLower();
                switch (error)
                {
                    case ("missing-input-secret"):
                        ModelState.AddModelError("", "The secret parameter is missing.");
                        break;
                    case ("invalid-input-secret"):
                        ModelState.AddModelError("", "The secret parameter is invalid or malformed.");
                        break;
                    case ("missing-input-response"):
                        ModelState.AddModelError("", "The response parameter is missing. Please, preceed with reCAPTCHA.");
                        break;
                    case ("invalid-input-response"):
                        ModelState.AddModelError("", "The response parameter is invalid or malformed.");
                        break;
                    default:
                        ModelState.AddModelError("", "Error occured. Please try again");
                        break;
                }
                return false;
            }

            return true;
        }

    }
}
