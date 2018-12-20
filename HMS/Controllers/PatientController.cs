using HMS.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using DataTables.Mvc;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        public ActionResult Appoitnments()
        {
            return View();
        }
        public ActionResult Patients()
        {
            return View();
        }
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

   
        public ActionResult LoadData()
        {

            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            //Find Order Column
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            if(sortColumn=="")
            {
                sortColumn = "PatientID";
            }
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            using(var dc = new HospitalManagementSystemEntities1())
            {
                // dc.Configuration.LazyLoadingEnabled = false; // if your table is relational, contain foreign key
                var v = (from a in dc.Patients select a);

                //SORT
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    v = v.OrderBy(sortColumn + " " + sortColumnDir);
                }

                recordsTotal = v.Count();
                var data = v.Skip(skip).Take(pageSize).ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var TodayDate = DateTime.Now.Date;
            IQueryable<Patient> query = db.Patients;
                var totalCount = query.Count();
            #region Filtering
            // Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.Name.Contains(value)
                                   );
            }

            var filteredCount = query.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            query = query.OrderBy(orderByString == string.Empty ? "PatientID asc" : orderByString);

            #endregion Sorting

            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);

            var data = query.Select(a => new
            {
                PatientID = a.PatientID,
                Name = a.Name,
                DOB = a.DOB.ToString(),
                EmployeeNumber = a.EmployeeNumber,
                RegistrationDate = a.RegistrationDate.ToString(),
                Gender=a.Gender
            }).ToList();
            //var data = query.ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
        }

        public ActionResult MakeAppointment(int id)
        {
            var model = db.Patients.Where(p => p.PatientID == id).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public void MakeAppointment(FormCollection fc)
        {
            Appointment apt = new Appointment();
            apt.PatientID = Convert.ToInt32(fc["PId"]);
            apt.DoctorID = 1;
            apt.Date = fc["ADate"];
            apt.Time = fc["ATime"];
            db.Appointments.Add(apt);
            db.SaveChanges();
        }


        public ActionResult GetAppointments([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var TodayDate = DateTime.Now.Date;
            IQueryable<Appointment> query = db.Appointments;
            var totalCount = query.Count();
            #region Filtering
            //Apply filters for searching
            if (requestModel.Search.Value != string.Empty)
                {
                    var value = requestModel.Search.Value.Trim();
                    query = query.Where(p => p.Time.Contains(value)
                                       );
                }

            var filteredCount = query.Count();

            #endregion Filtering

            #region Sorting
            // Sorting
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            query = query.OrderBy(orderByString == string.Empty ? "AppointmentID asc" : orderByString);

            #endregion Sorting

            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);


            var data = query.Select(a => new {
                PatientName = a.Patient.Name,
                DoctorName =a.User.UserName,
                Date=a.Date,
                Time=a.Time,
                AppointmentID=a.AppointmentID
            }).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
        }

    }



    internal class User
    {
        public String FirstName { get; set; }
        public String LastName{ get; set; }
    }
}