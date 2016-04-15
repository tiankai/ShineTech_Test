using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TaskTest_Web.Models;
using System.Web.Routing;

namespace TaskTest_Web.Controllers
{

    public class AccountController : Controller
    {
        private const string CookieName = "UName";

        private IUserAuth _userAuth;

        protected override void Initialize(RequestContext requestContext)
        {
            _userAuth = UserAuth.GetInstance();

            base.Initialize(requestContext);
        }
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return ContextDependentView();
        }

        private string CalcHash(string text)
        {
            //TODO: 哈希运算

            return text;
        }

        private string SetUserCookie(LogOnModel model, int expiredHour)
        {
            string hashName = CalcHash(string.Concat(model.UserName, model.Password));
            // 
            var cookie = new HttpCookie(CookieName, hashName);
            cookie.Expires = DateTime.Now.AddHours(expiredHour);
            HttpContext.Response.SetCookie(cookie);

            return hashName;
        }
        //
        // POST: /Account/JsonLogOn
        [HttpPost]
        public JsonResult JsonLogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                int userId = _userAuth.UserLogin(model);
                if (userId > 0)
                {
                    model.UserId = userId;
                    string hashName = SetUserCookie(model, 6);                    
                    HttpContext.Session.Add(hashName, model);

                    return Json(new { success = true, redirect = returnUrl });
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                int userId = _userAuth.UserLogin(model);
                if (userId > 0)
                {
                    model.UserId = userId;
                    string hashName = SetUserCookie(model, 6);   
                    HttpContext.Session.Add(hashName, model);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            var userCookies = HttpContext.Request.Cookies;
            var userCookie = userCookies.Get(CookieName);
            // 
            if(userCookie != null)
            {
                string hashName = userCookie.Value;
                // 
                if (string.IsNullOrEmpty(hashName) == false)
                {
                    HttpContext.Session.Remove(hashName);
                }
            }            

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return ContextDependentView();
        }

        //
        // POST: /Account/JsonRegister
        [HttpPost]
        public ActionResult JsonRegister(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                bool flag = _userAuth.RegisterUser(model);              
                if (flag ==true)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return Json(new { success = true });
                }
                else
                {
                    ModelState.AddModelError("", "注册失败!");
                }
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        //
        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                bool flag = _userAuth.RegisterUser(model);
                if (flag == true)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "注册失败!");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }       

        public string GetUserList(string name)
        {
            var users = _userAuth.GetUserList(name);

            var jsonUsers = this.Json(users);

            string jsonStr = jsonUsers.Data.ToString();

            return jsonStr;
        }

        private ActionResult ContextDependentView()
        {
            string actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = string.Concat("Json", actionName);
                return PartialView(string.Concat(actionName, "Partial"));
            }
            else
            {
                ViewBag.FormAction = actionName;
                return View();
            }
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors
                .Select(error => error.ErrorMessage));
        }

    }
}
