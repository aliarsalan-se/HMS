using HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        //Global Variables
        private HospitalManagementSystemEntities1 db = new HospitalManagementSystemEntities1();

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
        [HttpPost]
        public ContentResult AddPatient(FormCollection fc, HttpPostedFileBase Img)
        {
            Patient pt=new Patient();
            pt.Name = fc["Name"];
            pt.DOB = fc["DOB"];
            pt.Gender = fc["Gender"];
            pt.RegistrationDate = fc["RegistrationDate"];
            pt.PhoneNumber = fc["PhoneNumber"];
            pt.Adress = fc["Address"];
            //Code for Image
            int id = db.Patients.Count();
            id = id + 1;
            string UploadPath = Server.MapPath("~/Uploads/Patients");
            Img.SaveAs(System.IO.Path.Combine(UploadPath,id.ToString(),".jpeg"));
            pt.image = "~/Uploads/Patients" + "/" + id.ToString() + ".jpeg";
            db.Patients.Add(pt);
            db.SaveChanges();
          
            return Content(pt.Name);

        }
    }
  
   

    internal class User
    {
        public String FirstName { get; set; }
        public String LastName{ get; set; }
    }
}