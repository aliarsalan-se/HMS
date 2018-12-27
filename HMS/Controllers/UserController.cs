using HMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HMS.Controllers
{
    public class UserController : Controller
    {
        private HospitalManagementSystemEntities1 db = new HospitalManagementSystemEntities1();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult AddUser()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddUser(User u, HttpPostedFileBase Image)
        {
            User user = new User();
            user.UserName = u.UserName;
            user.EmployeeNumber = u.EmployeeNumber;
            user.PhoneNum = u.PhoneNum;
            user.Role = u.Role;
            user.EmailAddress = u.EmailAddress;
            user.Password = u.Password;
            if(Image != null)
            {
                int id = db.Users.Count();
                id = id + 1;
                string UploadPath = Server.MapPath("/Uploads/Users");
                string filename = id.ToString() + ".jpeg";
                Image.SaveAs(System.IO.Path.Combine(UploadPath,filename));
                user.Image = "/Uploads/Users" + "/" + filename;
            }
            db.Users.Add(user);
            db.SaveChanges();
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(User u)
        {
            // this action is for handle post (login)
            if (ModelState.IsValid) // this is check validity
            {
                using (db)
                {
                    var v = db.Users.Where(a => a.EmailAddress.Equals(u.EmailAddress) && a.Password.Equals(u.Password)).FirstOrDefault();
                    if (v != null)
                    {
                        Session["LogedUserID"] = v.ID.ToString();
                        Session["LogedUserFullname"] = v.UserName.ToString();
                        Session["Role"] = v.Role.ToString();
                        if (v.Image != null)
                            Session["Picture"] = v.Image.ToString();
                        return RedirectToAction("Patients","Patient");
                    }


                }
            }
            ViewBag.Message = "          Invalid UserName or Password ";
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