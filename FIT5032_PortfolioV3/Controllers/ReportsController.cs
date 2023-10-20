using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_PortfolioV3.Models;
using Microsoft.AspNet.Identity;

namespace FIT5032_PortfolioV3.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: Reports
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Staff"))
            {
                // Display appointments entered by the logged-in staff user
                var reports = db.Reports.Where(a => a.Appointment.StaffUserId == userId);
                return View(reports.ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                // Display appointments entered by the logged-in patient user
                var reports = db.Reports.Where(a => a.Appointment.PatientUserId == userId);
                return View(reports.ToList());
            }
            else if (User.IsInRole("Admin"))
            {
                // Display all appointments for admins
                var reports = db.Reports.Include(r => r.Appointment);
                return View(reports.ToList());
            }
            return View();

        }

        // GET: Reports/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId), "Id", "Description", report.AppointmentId);
            return View(report);
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            bool hasAppointment = db.Appointments.Any(a => a.StaffUserId == userId);

            if (!hasAppointment)
            {
                // If no appointment is found, redirect the user to the Index page with a warning message.
                TempData["Message"] = "You cannot write report as you have no appointments!";
                return RedirectToAction("Index");
            }

            // Get a list of AppointmentIds that have already been rated
            var reportedAppointmentIds = db.Reports.Select(r => r.AppointmentId).ToList();

            // Filter appointments for the current user that have not been rated
            var unratedAppointments = db.Appointments.Where(a => a.PatientUserId == userId && !reportedAppointmentIds.Contains(a.Id)).ToList();

            if (!unratedAppointments.Any())
            {
                TempData["Message"] = "You cannot rate as all your appointments have been rated!";
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId && !reportedAppointmentIds.Contains(a.Id)), "Id", "AppointmentDateTime");
            return View();
        }

        [Authorize(Roles = "Admin,Staff")]
        // POST: Reports/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,Date,Time,AppointmentId")] Report report)
        {
            var userId = User.Identity.GetUserId();
            if (db.Ratings.Any(r => r.AppointmentId == report.AppointmentId))
            {
                ModelState.AddModelError("", "This appointment has already been reported.");
                
                ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId), "Id", "AppointmentDateTime", report.AppointmentId);
                return View(report);
            }
            report.Id = Guid.NewGuid().ToString();
            report.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            report.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");

            ModelState.Clear();
            TryValidateModel(report);
            if (ModelState.IsValid)
            {
                db.Reports.Add(report);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime", report.AppointmentId);
            return View(report);
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            //var userId = User.Identity.GetUserId();
            //ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime", report.AppointmentId);
            return View(report);
        }

        [Authorize(Roles = "Admin,Staff")]
        // POST: Reports/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Description,Date,Time,AppointmentId")] Report report)
        {
            report.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            report.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            if (ModelState.IsValid)
            {
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(report);
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = db.Reports.Find(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        [Authorize(Roles = "Admin")]
        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Report report = db.Reports.Find(id);
            db.Reports.Remove(report);
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
