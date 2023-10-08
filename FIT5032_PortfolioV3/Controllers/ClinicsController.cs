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
    public class ClinicsController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: Clinics
        public ActionResult Index()
        {
            return View(db.Clinics.ToList());
        }

        // GET: Clinics/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinics clinics = db.Clinics.Find(id);
            if (clinics == null)
            {
                return HttpNotFound();
            }
            return View(clinics);
        }

        // GET: Clinics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clinics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,PhoneNo,ddressDetail,Postcode")] Clinics clinics)
        {
            clinics.Id = User.Identity.GetUserId();
            ModelState.Clear();
            TryValidateModel(clinics);
            if (ModelState.IsValid)
            {
                db.Clinics.Add(clinics);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clinics);
        }

        // GET: Clinics/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinics clinics = db.Clinics.Find(id);
            if (clinics == null)
            {
                return HttpNotFound();
            }
            return View(clinics);
        }

        // POST: Clinics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,PhoneNo,ddressDetail,Postcode")] Clinics clinics)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clinics).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(clinics);
        }

        // GET: Clinics/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinics clinics = db.Clinics.Find(id);
            if (clinics == null)
            {
                return HttpNotFound();
            }
            return View(clinics);
        }

        // POST: Clinics/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Clinics clinics = db.Clinics.Find(id);
            db.Clinics.Remove(clinics);
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
