using HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class DoctorsController : Controller
    {
        private HospitalManagementSystemEntities1 db = new HospitalManagementSystemEntities1();
        // GET: Doctors
        [HttpGet]
       public JsonResult GetDoctors()
        {
            IEnumerable<User> modelList = new List<User>();
            var  dr = db.Users.ToList();
            modelList = dr.Select(x =>
                     new User()
                     {
                         ID = x.ID,
                         UserName = x.UserName
                     });
            
            return Json(modelList, JsonRequestBehavior.AllowGet);
        }
    }
}