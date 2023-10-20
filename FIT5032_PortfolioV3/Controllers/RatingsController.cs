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

namespace FIT5032_PortfolioV3.Controllers
{
    [Authorize]
    public class RatingsController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: Ratings
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Staff"))
            {
                // Display appointments entered by the logged-in staff user
                var ratings = db.Ratings.Where(a => a.Appointment.StaffUserId == userId);
                return View(ratings.ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                // Display appointments entered by the logged-in patient user
                var ratings = db.Ratings.Where(a => a.Appointment.PatientUserId == userId);
                return View(ratings.ToList());
            }
            else if (User.IsInRole("Admin"))
            {
                // Display all appointments for admins
                var ratings = db.Ratings.Include(r => r.Appointment);
                return View(ratings.ToList());
            }
            return View();
        }

        // GET: Ratings/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            //ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId, "Id", "AppointmentDateTime", rating.AppointmentId);
            return View(rating);
        }

        // GET: Ratings/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            bool hasAppointment = db.Appointments.Any(a => a.PatientUserId == userId);

            if (!hasAppointment)
            {
                // If no appointment is found, redirect the user to the Index page with a warning message.
                TempData["Message"] = "You cannot rate as you have no appointments!";
                return RedirectToAction("Index");
            }
            // Get a list of AppointmentIds that have already been rated
            var ratedAppointmentIds = db.Ratings.Select(r => r.AppointmentId).ToList();

            // Filter appointments for the current user that have not been rated
            var unratedAppointments = db.Appointments.Where(a => a.PatientUserId == userId && !ratedAppointmentIds.Contains(a.Id)).ToList();

            if (!unratedAppointments.Any())
            {
                TempData["Message"] = "You cannot rate as all your appointments have been rated!";
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId && !ratedAppointmentIds.Contains(a.Id)), "Id", "AppointmentDateTime");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Create([Bind(Include = "Id,Description,Rate,Date,Time,AppointmentId")] Rating rating)
        {
            var userId = User.Identity.GetUserId();
            // Check if a rating for this appointment already exists
            if (db.Ratings.Any(r => r.AppointmentId == rating.AppointmentId))
            {
                ModelState.AddModelError("", "This appointment has already been rated.");
                
                ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId), "Id", "AppointmentDateTime", rating.AppointmentId);
                return View(rating);
            }

            rating.Id = Guid.NewGuid().ToString();
            rating.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            rating.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            ModelState.Clear();
            TryValidateModel(rating);
            if (ModelState.IsValid)
            {
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId), "Id", "AppointmentDateTime", rating.AppointmentId);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            //var userId = User.Identity.GetUserId();
            //ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.PatientUserId == userId), "Id", "AppointmentDateTime", rating.AppointmentId);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Edit([Bind(Include = "Id,Description,Rate,Date,Time,AppointmentId")] Rating rating)
        {
           rating.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
           rating.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            if (ModelState.IsValid)
            {
                
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
                string mess = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log or print the error message to identify the specific issue
                        mess = mess + error.ErrorMessage;
                    }
                }
                TempData["Message"] = mess;
                return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult DeleteConfirmed(string id)
        {
            Rating rating = db.Ratings.Find(id);
            db.Ratings.Remove(rating);
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
