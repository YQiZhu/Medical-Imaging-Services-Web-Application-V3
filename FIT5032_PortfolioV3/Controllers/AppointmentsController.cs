using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using FIT5032_PortfolioV3.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SendGrid.Helpers.Mail;
using SendGrid;

namespace FIT5032_PortfolioV3.Controllers
{
    public class AppointmentsController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: Appointments
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            if (User.IsInRole("Staff"))
            {
                // Display appointments entered by the logged-in staff user
                var appointments = db.Appointments.Where(a => a.StaffId.Id == userId).Include(a => a.PatientId).Include(a => a.Clinics);
                return View(appointments.ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                // Display appointments entered by the logged-in patient user
                var appointments = db.Appointments.Where(a => a.PatientId.Id == userId).Include(a => a.StaffId).Include(a => a.Clinics);
                return View(appointments.ToList());
            }
            else if (User.IsInRole("Admin"))
            {
                // Display all appointments for admins
                var appointments = db.Appointments.Include(a => a.StaffId).Include(a => a.PatientId).Include(a => a.Clinics);
                return View(appointments.ToList());
            }

            // If the user doesn't have any of the specified roles, return an empty view
            return View();
        }

        // GET: Appointments/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointments appointments = db.Appointments.Find(id);
            if (appointments == null)
            {
                return HttpNotFound();
            }
            return View(appointments);
        }

        // GET: Appointments/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            // Find the "Staff" role
            var staffRole = roleManager.FindByName("Staff");

            // Get the user IDs in the "Staff" role
            var staffUserIds = staffRole.Users.Select(r => r.UserId).ToList();

            if (User.IsInRole("Patient"))
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName");
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            }
            else
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName");
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            }
            
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Create([Bind(Include = "Id,RoomNo,Date,Time,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            appointments.Id = Guid.NewGuid().ToString();
            ModelState.Clear();
            if (!IsValidTime(appointments.Time))
            {
                ModelState.AddModelError("Time", "Please select vaild working time (8:00 - 18:00)");
            }
            if (IsDateInPast(appointments.Date))
            {
                ModelState.AddModelError("Date", "Please select vaild date, the selected date cannot be in the past");
            }
            TryValidateModel(appointments);
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointments);
                db.SaveChanges();

                const String API_KEY = "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";
                try
                { 
                    var client = new SendGridClient(API_KEY);
                    var from = new EmailAddress("zhuyanqi001215@gmail.com", "FIT5032 Example Email User");
                    var toPatient = new EmailAddress(db.AspNetUsers.Find(appointments.PatientUserId).Email, db.AspNetUsers.Find(appointments.PatientUserId).FullName);
                    var toStaff = new EmailAddress(db.AspNetUsers.Find(appointments.StaffUserId).Email, db.AspNetUsers.Find(appointments.StaffUserId).FullName);
                    var plainTextContent = "Your Appointment book successfully! \nHere is your appointment information: Appointment " + appointments.Date + " " + appointments.Time + " at " + db.Clinics.Find(appointments.ClinicId).Name;
                    var htmlContent = "<p>" + plainTextContent + "</p>";
                    var msg = MailHelper.CreateSingleEmail(from, toPatient, "Your Appointment book successfully", plainTextContent, htmlContent);
                    var response = client.SendEmailAsync(msg).Result;// Send the email to Patient
                    msg = MailHelper.CreateSingleEmail(from, toStaff, "Your have a new Appointment", plainTextContent, htmlContent);
                }
                catch
                {
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.PatientUserId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.StaffUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);

            return View(appointments);
        }

        private bool IsValidTime(string time)
        {
            if (TimeSpan.TryParse(time, out TimeSpan parsedTime))
            {
                TimeSpan startTime = new TimeSpan(8, 0, 0);
                TimeSpan endTime = new TimeSpan(18, 0, 0);

                if (parsedTime >= startTime && parsedTime <= endTime)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDateInPast(String date)
        {
            if (DateTime.TryParse(date, out DateTime parsedDate))
            {
                return parsedDate <= DateTime.Today;
            }
            return false;
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointments appointments = db.Appointments.Find(id);
            if (appointments == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));

            // Find the "Staff" role
            var staffRole = roleManager.FindByName("Staff");

            // Get the user IDs in the "Staff" role
            var staffUserIds = staffRole.Users.Select(r => r.UserId).ToList();
            if (User.IsInRole("Patient"))
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName");
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            }
            else
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName");
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            }
            return View(appointments);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Edit([Bind(Include = "Id,RoomNo,Date,Time,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            if (!IsValidTime(appointments.Time))
            {
                ModelState.AddModelError("Time", "Please select vaild working time (8:00 - 18:00)");
            }
            if (IsDateInPast(appointments.Date))
            {
                ModelState.AddModelError("Date", "Please select vaild date, the selected date cannot be in the past");
            }
            if (ModelState.IsValid)
            {
                db.Entry(appointments).State = EntityState.Modified;
                db.SaveChanges();
                const String API_KEY = "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";
                try
                {
                    var client = new SendGridClient(API_KEY);
                    var from = new EmailAddress("zhuyanqi001215@gmail.com", "FIT5032 Example Email User");
                    var toPatient = new EmailAddress(db.AspNetUsers.Find(appointments.PatientUserId).Email, db.AspNetUsers.Find(appointments.PatientUserId).FullName);
                    var toStaff = new EmailAddress(db.AspNetUsers.Find(appointments.StaffUserId).Email, db.AspNetUsers.Find(appointments.StaffUserId).FullName);
                    var plainTextContent = "Your Appointment Change successfully! \nHere is your appointment information: " + appointments.AppointmentDateTime;
                    var htmlContent = "<p>" + plainTextContent + "</p>";
                    var msg = MailHelper.CreateSingleEmail(from, toPatient, "Your Appointment change successfully", plainTextContent, htmlContent);
                    var response = client.SendEmailAsync(msg).Result;// Send the email to Patient
                    msg = MailHelper.CreateSingleEmail(from, toStaff, "Your have a change of Appointment", plainTextContent, htmlContent);
                }
                catch
                {
                    return View();
                }
                return RedirectToAction("Index");
            }
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.PatientUserId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.StaffUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
            return View(appointments);
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Appointments appointments = db.Appointments.Find(id);
            if (appointments == null)
            {
                return HttpNotFound();
            }
            return View(appointments);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult DeleteConfirmed(string id)
        {
            Appointments appointments = db.Appointments.Find(id);
            db.Appointments.Remove(appointments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
