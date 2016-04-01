using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TaskTest_Web.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public string Test()
        {
            return "{ \"Success\" : true, \"Result\" : [1, 2, 3, 2, 4] }";
        }

        public ActionResult NewTask()
        {
            return View();
        }
    }
}
