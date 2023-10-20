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
            UpdateClinicRatings();
            return View(db.Clinics.ToList());
        }

        public void UpdateClinicRatings()
        {
            var clinics = db.Clinics.Include("Appointments").ToList(); // Retrieve all clinics with their appointments
            foreach (var clinic in clinics)
            {
                // Get all ratings for appointments belonging to this clinic
                var ratings = clinic.Appointments.SelectMany(a => a.Ratings).ToList();

                // Calculate the average rating
                if (ratings.Count > 0)
                {
                    clinic.AverageRate = (decimal)ratings.Average(r => r.Rate);
                }
                else
                {
                    clinic.AverageRate = 0; // Default value if there are no ratings
                }
            }
            db.SaveChanges(); // Save changes to the database
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
        public ActionResult Create([Bind(Include = "Id,Name,PhoneNo,AddressDetail,Postcode,Description,Latitude,Longitude,AverageRate")] Clinics clinics)
        {
            clinics.Id = Guid.NewGuid().ToString();
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
        public ActionResult Edit([Bind(Include = "Id,Name,PhoneNo,AddressDetail,Postcode,Description,Latitude,Longitude,AverageRate")] Clinics clinics)
        {
            clinics.AverageRate = 0;
            if (ModelState.IsValid)
            {
                db.Entry(clinics).State = EntityState.Modified;
                db.SaveChanges();
                UpdateClinicRatings();
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

        public Dictionary<string, int> GetAppointmentsPerClinic()
        {
            var data = db.Clinics.Include(c => c.Appointments)
                         .Select(c => new { c.Name, Count = c.Appointments.Count })
                         .ToDictionary(c => c.Name, c => c.Count);
            return data;
        }

        public ActionResult Chart()
        {
            var data = GetAppointmentsPerClinic();
            return View(data);
        }



        public ActionResult ExportToCsv()
        {
            var clinics = db.Clinics.ToList();
            string csvData = ConvertToCsv(clinics);

            var bytes = System.Text.Encoding.UTF8.GetBytes(csvData);
            var stream = new System.IO.MemoryStream(bytes);

            return new FileStreamResult(stream, "text/csv") { FileDownloadName = "Clinics.csv" };
        }

        private string ConvertToCsv(List<Clinics> clinics)
        {
            string csvData = "Name,PhoneNo,AddressDetail,Postcode,Description,Latitude,Longitude,AverageRate\n";

            foreach (var clinic in clinics)
            {
                //\" is an escaped double quote in C#.
                //special characters (like commas) need to be wrapped in double quotes to be correctly interpreted as a single field in the CSV.
                csvData += $"{clinic.Name},{clinic.PhoneNo},\"{clinic.AddressDetail.Replace("\"", "\"\"")}\",{clinic.Postcode},{clinic.Description},{clinic.Latitude},{clinic.Longitude},{clinic.AverageRate}\n";
            }

            return csvData;
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
