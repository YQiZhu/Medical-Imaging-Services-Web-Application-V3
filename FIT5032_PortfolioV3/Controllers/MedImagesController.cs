using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_PortfolioV3.Models;
using Microsoft.AspNet.Identity;
using SendGrid.Helpers.Mail;
using SendGrid;
using FIT5032_PortfolioV3.Utils;
using System.Web.Helpers;
using System.Web.UI.WebControls;

namespace FIT5032_PortfolioV3.Controllers
{
    [Authorize]
    public class MedImagesController : Controller
    {
        private FIT5032_Model db = new FIT5032_Model();

        // GET: MedImages
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Staff"))
            {
                // Display appointments entered by the logged-in staff user
                var medImages = db.MedImages.Where(a => a.Appointment.StaffUserId == userId);
                
                return View(medImages.ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                // Display appointments entered by the logged-in patient user
                var medImages = db.MedImages.Where(a => a.Appointment.PatientUserId == userId);
                return View(medImages.ToList());
            }
            else if (User.IsInRole("Admin"))
            {
                // Display all appointments for admins
                var medImages = db.MedImages.Include(m => m.Appointment);
                return View(medImages.ToList());
            }
            
            //var medImages = db.MedImages.Include(m => m.Appointment);
            return View();
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
            ViewBag.AppointmentId = db.Appointments.Find(medImage.AppointmentId).AppointmentDateTime;
            return View(medImage);
        }

        // GET: MedImages/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            bool hasAppointment = db.Appointments.Any(a => a.StaffUserId == userId);
            bool isAdmin = User.IsInRole("Admin");
            if (!hasAppointment && !isAdmin)
            {
                // If no appointment is found, redirect the user to the Index page with a warning message.
                TempData["Message"] = "You cannot add Medcial Images as you have no appointments!";
                return RedirectToAction("Index");
            }
            // Get a list of AppointmentIds that have already been rated
            var images = db.MedImages.Select(r => r.AppointmentId).ToList();

            // Filter appointments for the current user that have not been rated
            var unupload = db.Appointments.Where(a => a.StaffUserId == userId && !images.Contains(a.Id)).ToList();

            if (!unupload.Any() && !isAdmin)
            {
                TempData["Message"] = "You cannot add Medcial Images as all your appointments have been added!";
                return RedirectToAction("Index");
            }
            if (User.IsInRole("Admin"))
                { ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "AppointmentDateTime"); }
            else {ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime"); }
            
            
            return View();
        }

        // POST: MedImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Time,AppointmentId")] MedImage medImage, HttpPostedFileBase postedFile)
        {
            medImage.Id = Guid.NewGuid().ToString(); ;
            medImage.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
            medImage.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");
            ModelState.Clear();
            if (postedFile != null)
            {
                var myUniqueFileName = string.Format(@"{0}", Guid.NewGuid());
                medImage.Path = myUniqueFileName;
            }
            TryValidateModel(medImage);
            if (ModelState.IsValid)
            {
                string serverPath = Server.MapPath("~/Uploads/");
                string fileExtension = Path.GetExtension(postedFile.FileName);
                string filePath = medImage.Path + fileExtension;
                medImage.Path = filePath;
                postedFile.SaveAs(serverPath + medImage.Path);


                db.MedImages.Add(medImage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
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
                var userId = User.Identity.GetUserId();
                ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime", medImage.AppointmentId);
                return View(medImage);
            }
        }

        /*
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
            var userId = User.Identity.GetUserId();
            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime", medImage.AppointmentId);
            if (User.IsInRole("Admin"))
            { ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "AppointmentDateTime"); }
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
                medImage.Date = DateTime.Now.Date.ToString("yyyy-MM-dd");
                medImage.Time = DateTime.Now.TimeOfDay.ToString("hh\\:mm");
                db.Entry(medImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var userId = User.Identity.GetUserId();
            ViewBag.AppointmentId = new SelectList(db.Appointments.Where(a => a.StaffUserId == userId), "Id", "AppointmentDateTime", medImage.AppointmentId);
            if (User.IsInRole("Admin"))
            { ViewBag.AppointmentId = new SelectList(db.Appointments, "Id", "AppointmentDateTime"); }
            return View(medImage);
        }
        */

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
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            MedImage medImage = db.MedImages.Find(id);
            db.MedImages.Remove(medImage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult SendEmail(string id)
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
            Appointments appointment = db.Appointments.Find(medImage.AppointmentId);

            var emailModel = new SendEmailViewModel
            {
                ToEmail = User.Identity.Name, // Default recipient email
                Subject = appointment.AppointmentDateTime,
                Contents = "Here is your X-ray image from X-rayConnect.",
                AttachmentPath = medImage.Path // Attachment path
            };
            return View(emailModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult SendEmail(SendEmailViewModel model)
        {
            const String API_KEY = "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";
            //string mes = model.ToEmail + model.Subject + model.Subject;
            if (ModelState.IsValid)
            {
                try
                {
                var client = new SendGridClient(API_KEY);
                var from = new EmailAddress("zhuyanqi001215@gmail.com", "FIT5032 Example Email User");
                var to = new EmailAddress(model.ToEmail, model.ToEmail);
                var plainTextContent = model.Contents;
                var htmlContent = "<p>" + model.Contents + "</p>";
                var msg = MailHelper.CreateSingleEmail(from, to, model.Subject, plainTextContent, htmlContent);
                
                if (!string.IsNullOrEmpty(model.AttachmentPath))
                {
                        string path = "~/Uploads/" + model.AttachmentPath;
                        string serverPath = Server.MapPath(path);
                        msg.AddAttachment(model.AttachmentPath, Convert.ToBase64String(System.IO.File.ReadAllBytes(serverPath)));
                    }
                    var response = client.SendEmailAsync(msg).Result;// Send the email
                if (response != null)
                {
                    TempData["Message"] = "Email sent successfully.";
                        ModelState.Clear();
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                string mess = "";
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        // Log or print the error message to identify the specific issue
                        mess=mess+error.ErrorMessage;
                    }
                }
                TempData["Message"] = mess;
            // If ModelState is not valid, redisplay the form with validation errors
            return RedirectToAction("Index");
            }
            TempData["Message"] = "Email sent Fail.";
            // If ModelState is not valid, redisplay the form with validation errors
            return View();
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
