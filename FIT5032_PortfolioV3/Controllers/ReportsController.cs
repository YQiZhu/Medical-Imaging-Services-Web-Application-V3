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
            var reports = db.Reports.Include(r => r.Appointment);
            return View(reports.ToList());
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
            return View(report);
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "AppointmentDateTime");
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
            report.Id = Guid.NewGuid().ToString(); ;
            ModelState.Clear();
            TryValidateModel(report);
            if (ModelState.IsValid)
            {
                db.Reports.Add(report);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", report.AppointmentId);
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
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", report.AppointmentId);
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
            if (ModelState.IsValid)
            {
                db.Entry(report).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", report.AppointmentId);
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
