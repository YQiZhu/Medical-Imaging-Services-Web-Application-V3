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
    public class MedImagesController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: MedImages
        public ActionResult Index()
        {
            var medImages = db.MedImages.Include(m => m.Appointment);
            return View(medImages.ToList());
        }

        // GET: MedImages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedImage medImage = db.MedImages.Find(id);
            if (medImage == null)
            {
                return HttpNotFound();
            }
            return View(medImage);
        }

        // GET: MedImages/Create
        public ActionResult Create()
        {
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "AppointmentDateTime");
            return View();
        }

        // POST: MedImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Time,AppointmentId")] MedImage medImage)
        {
            medImage.Id = Guid.NewGuid().ToString(); ;
            ModelState.Clear();
            TryValidateModel(medImage);
            if (ModelState.IsValid)
            {
                db.MedImages.Add(medImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", medImage.AppointmentId);
            return View(medImage);
        }

        // GET: MedImages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedImage medImage = db.MedImages.Find(id);
            if (medImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", medImage.AppointmentId);
            return View(medImage);
        }

        // POST: MedImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Time,AppointmentId")] MedImage medImage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(medImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "Description", medImage.AppointmentId);
            return View(medImage);
        }

        // GET: MedImages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MedImage medImage = db.MedImages.Find(id);
            if (medImage == null)
            {
                return HttpNotFound();
            }
            return View(medImage);
        }

        // POST: MedImages/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MedImage medImage = db.MedImages.Find(id);
            db.MedImages.Remove(medImage);
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
