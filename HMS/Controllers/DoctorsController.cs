using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class DoctorsController : Controller
    {
        // GET: Doctors
        [HttpGet]
       public JsonResult GetDoctors()
        {
            var doctors = new List<string>();
            doctors.Add("Peter");
            doctors.Add("John Snow");
            doctors.Add("Oliver");
            return Json(doctors, JsonRequestBehavior.AllowGet);
        }
    }
}