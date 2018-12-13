using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
      public ActionResult AddPatient()
        {
            return View();
        }
     [HttpPost]
        public JsonResult Test(string Emp)
        {

            User user = new User();
            user.FirstName = "10/02/1995";
            user.LastName = "Male";
           

            return Json(user, JsonRequestBehavior.AllowGet);
        }
    }

    internal class User
    {
        public String FirstName { get; set; }
        public String LastName{ get; set; }
    }
}