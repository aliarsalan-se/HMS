using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }



        [HttpGet]
        public JsonResult Roles()
        {
            var roles = new List<string>();
            roles.Add("Doctor");
            roles.Add("Staff");
            return Json(roles, JsonRequestBehavior.AllowGet);
        }
    }
}