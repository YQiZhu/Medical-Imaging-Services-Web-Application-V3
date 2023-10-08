using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_PortfolioV3.Models;

namespace FIT5032_PortfolioV3.Controllers
{
    public class AppointmentsController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: Appointments
        public ActionResult Index()
        {
            var appointments = db.Appointments.Include(a => a.AspNetUsers).Include(a => a.AspNetUsers1).Include(a => a.Clinics);
            return View(appointments.ToList());
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
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            return View();
        }

        // POST: Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Description,RoomNo,Date,Time,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
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
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.PatientUserId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "Email", appointments.StaffUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
            return View(appointments);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
