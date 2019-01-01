using DevExpress.Web.Mvc;
using HMS.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using DataTables.Mvc;
using System.Collections.Generic;
using HMS.Reports;
using System.IO;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        //Global Variables
        private HospitalManagementSystemEntities1 db = new HospitalManagementSystemEntities1();
        private int DiagnoseId=0;
        public ActionResult CreatePatient()
        {
            if (Session["LogedUserID"] == null)
                return RedirectToAction("Login", "User");
            return View();
        }
        public ActionResult Appointments()
        {
            if (Session["LogedUserID"] == null)
                return RedirectToAction("Login", "User");
            return View();
        }
        public ActionResult Patients()
        {
            return View();
        }
        // GET: Patient
        public ActionResult AddPatient()
        {
            if (Session["LogedUserID"] == null)
                return RedirectToAction("Login", "User");
            return View();
        }
        [HttpPost]
        public ContentResult AddPatient(FormCollection fc, HttpPostedFileBase Img)
        {
            Patient pt = new Patient();
            pt.Name = fc["Name"];
            pt.DOB = fc["DOB"];
            pt.Gender = fc["Gender"];
            pt.RegistrationDate = fc["RegistrationDate"];
            pt.PhoneNumber = fc["PhoneNumber"];
            pt.Adress = fc["Address"];
            //Code for Image
            if (Img != null)
            {
                int id = db.Patients.OrderByDescending(p => p.PatientID).First().PatientID;
                id = id + 1;
                string UploadPath = Server.MapPath("~/Uploads/Patients");
                Img.SaveAs(System.IO.Path.Combine(UploadPath, id.ToString(), ".jpeg"));
                pt.image = "~/Uploads/Patients" + "/" + id.ToString() + ".jpeg";
            }
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
            if (sortColumn == "")
            {
                sortColumn = "PatientID";
            }
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;
            using (var dc = new HospitalManagementSystemEntities1())
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
                Gender = a.Gender
            }).ToList();
            //var data = query.ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
        }
        public ActionResult MakeAppointment(int id)
        {
            if (Session["LogedUserID"] == null)
                return RedirectToAction("Login", "User");
            var model = db.Patients.Where(p => p.PatientID == id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public void MakeAppointment(FormCollection fc)
        {
            Appointment apt = new Appointment();
            apt.PatientID = Convert.ToInt32(fc["PId"]);
            apt.DoctorID = Convert.ToInt32(fc["DId"]);
            apt.Date = fc["ADate"];
            apt.Time = fc["ATime"];
            db.Appointments.Add(apt);
            db.SaveChanges();
        }
        public ActionResult GetAppointments([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

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


            var data = query.Select(a => new
            {
                PatientName = a.Patient.Name,
                DoctorName = a.User.UserName,
                Date = a.Date,
                Time = a.Time,
                AppointmentID = a.AppointmentID
            }).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Diagnose(int id)
        {
            if (Session["LogedUserID"] == null)
                return RedirectToAction("Login", "User");
            var model = db.Appointments.Where(p => p.AppointmentID == id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ContentResult GetDiagnose(FormCollection fc)
        {
            Diagnosi dg = new Diagnosi();
            dg.AppointmentID = Convert.ToInt32(fc["Appointment"]);
            dg.BloodPressure = fc["BloodPressure"];
            dg.Temperature = fc["Temperature"];
            dg.Symptoms = fc["Symptoms"];
            dg.DiagnosedProblem = fc["DiagnosedProblem"];
            db.Diagnosis.Add(dg);
            db.SaveChanges();
            int did = db.Diagnosis.OrderByDescending(p => p.ID).First().ID;
            return Content(did.ToString());
        }
        [HttpPost]
        public ActionResult SavePrescription(List<Prescription> data)
        {
             DiagnoseId = Convert.ToInt16(data[1].DiagnoseID);

            foreach (Prescription rec in data)
            {
                if (rec.Medicine != null)
                {
                    Prescription p = new Prescription();
                    p.Medicine = rec.Medicine;
                    p.Dosage = rec.Dosage;
                    p.Quantity = Convert.ToInt16(rec.Quantity);
                    p.Time = rec.Time;
                    p.DiagnoseID = Convert.ToUInt16(rec.DiagnoseID);
                    db.Prescriptions.Add(p);
                    db.SaveChanges();
                }
            }
            var model = db.Prescriptions.Where(p => p.DiagnoseID == DiagnoseId).ToList();
            
             return Json("Changes Successfully Made",JsonRequestBehavior.AllowGet);
        }
        public ActionResult Print()
        {
            var report = new XtraReport1();
            var stream = new MemoryStream();
            report.ExportToPdf(stream);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "filename.pdf",
                Inline = false,
            };
            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(stream.GetBuffer(), "application/pdf");
        }
    }
}