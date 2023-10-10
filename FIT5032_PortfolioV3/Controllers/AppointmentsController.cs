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
        public ActionResult Create([Bind(Include = "Id,Description,RoomNo,Date,Time,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            appointments.Id = Guid.NewGuid().ToString();
            ModelState.Clear();
            TryValidateModel(appointments);
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointments);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.PatientUserId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.StaffUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);

            return View(appointments);
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
        public ActionResult Edit([Bind(Include = "Id,Description,RoomNo,Date,Time,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(appointments).State = EntityState.Modified;
                db.SaveChanges();
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
