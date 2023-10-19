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
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Data.Entity.Validation;
using System.Diagnostics;

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
                var appointments = db.Appointments.Where(a => a.StaffId.Id == userId).Include(a => a.PatientId).Include(a => a.Clinics).Include(a => a.TimeSlot);
                return View(appointments.ToList());
            }
            else if (User.IsInRole("Patient"))
            {
                // Display appointments entered by the logged-in patient user
                var appointments = db.Appointments.Where(a => a.PatientId.Id == userId).Include(a => a.StaffId).Include(a => a.Clinics).Include(a => a.TimeSlot);
                return View(appointments.ToList());
            }
            else if (User.IsInRole("Admin"))
            {
                // Display all appointments for admins
                var appointments = db.Appointments.Include(a => a.StaffId).Include(a => a.PatientId).Include(a => a.Clinics).Include(a => a.TimeSlot);
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
                //ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name");
            }
            else
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName");
                // Display only users with the "Staff" role and their names
                //ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName");
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
        public ActionResult Create([Bind(Include = "Id,RoomNo,Date,TimeSlotId,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            
            ModelState.Clear();
            TryValidateModel(appointments);
           /*
            if (ModelState.IsValid)
            {
                db.Appointments.Add(appointments);
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

                
            }*/
            TempData["AppointmentData"] = appointments; // Store the model data in TempData
                return RedirectToAction("Select");
        }

        // GET: Appointments/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["AppointmentEditId"] = id;
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
            var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
                .Select(bookedSlot => bookedSlot.SlotId).ToList();
            if (User.IsInRole("Patient"))
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName", appointments.PatientUserId);
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName", appointments.StaffUserId);
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name", appointments.TimeSlotId);
            }
            else
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName");
                // Display only users with the "Staff" role and their names
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName", appointments.StaffUserId);
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name", appointments.TimeSlotId);
            }
            return View(appointments);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Edit([Bind(Include = "Id,RoomNo,Date,TimeSlotId,ClinicId,PatientUserId,StaffUserId")] Appointments appointments)
        {
            if (ModelState.IsValid)
            {

                //db.Entry(appointments).State = EntityState.Modified;
                //db.SaveChanges();

                /*const String API_KEY = "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";
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
                }*/
                TempData["AppointmentEdit"] = appointments;
                return RedirectToAction("EditSelect");
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
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                // Find the "Staff" role
                var staffRole = roleManager.FindByName("Staff");
                // Get the user IDs in the "Staff" role
                var staffUserIds = staffRole.Users.Select(r => r.UserId).ToList();
                var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
                .Select(bookedSlot => bookedSlot.SlotId).ToList();
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName", appointments.PatientUserId);
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffUserIds.Contains(a.Id)), "Id", "FullName", appointments.StaffUserId);
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name", appointments.TimeSlotId);
                return View(appointments);
            }
        }

        public ActionResult EditSelect()
        {
            var appointments = TempData["AppointmentEdit"] as Appointments;
            if (appointments == null)
            {
                return RedirectToAction("Edit");
            }
            else
            {
                var userId = User.Identity.GetUserId();
                var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
                .Select(bookedSlot => bookedSlot.SlotId).ToList();
                var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName", appointments.PatientUserId);
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName", appointments.StaffUserId);
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots, "SlotId", "Name", appointments.TimeSlotId);
                
                return View(appointments);
            }

            //var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
            //ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName");
            //return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult EditSelect(Appointments model)
        {
            ModelState.Clear();
            TempData["AppointmentEdit"] = model; // Store the model data in TempData
            return RedirectToAction("EditSelectTime");
        }

        public ActionResult EditSelectTime()
        {
            
            var appointments = TempData["AppointmentEdit"] as Appointments;
            appointments.Id = TempData["AppointmentEditId"] as String;
            if (appointments == null)
            {
                return RedirectToAction("Select");
            }
            var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
                .Select(bookedSlot => bookedSlot.SlotId).ToList();
             var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
           
            // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
            var userId = User.Identity.GetUserId();
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers.Where(a => a.Id == userId), "Id", "FullName", appointments.PatientUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName", appointments.StaffUserId);
            ViewBag.TimeSlotId = new SelectList(db.TimeSlots, "SlotId", "Name", appointments.TimeSlotId);
            
            return View(appointments); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult EditSelectTime(Appointments appointments)
        {
            //appointments.Id = Guid.NewGuid().ToString();
            ModelState.Clear();
            TryValidateModel(appointments);
            if (ModelState.IsValid)
            {
                try
                {
                    BookedSlot bookedSlot = db.BookedSlots.Find(appointments.Id);
                    bookedSlot.BookingId = appointments.Id;
                    bookedSlot.SlotId = appointments.TimeSlotId;
                    bookedSlot.StaffUserId = appointments.StaffUserId;
                    bookedSlot.Date = appointments.Date;
                    db.Entry(bookedSlot).State = EntityState.Modified;

                    db.Entry(appointments).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            var errorMessage = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
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

                    var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date).Select(bookedSlot => bookedSlot.SlotId).ToList();
                    ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name");

                    return View(appointments); // Return to the same view to display validation errors
                }

                const String API_KEY = "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";
                try
                {
                    var client = new SendGridClient(API_KEY);
                    var from = new EmailAddress("zhuyanqi001215@gmail.com", "FIT5032 Example Email User");
                    var toPatient = new EmailAddress(db.AspNetUsers.Find(appointments.PatientUserId).Email, db.AspNetUsers.Find(appointments.PatientUserId).FullName);
                    var toStaff = new EmailAddress(db.AspNetUsers.Find(appointments.StaffUserId).Email, db.AspNetUsers.Find(appointments.StaffUserId).FullName);
                    var plainTextContent = "Your Appointment Change successfully! \nHere is your appointment information: " + appointments.Date + " " + appointments.TimeSlot + " at " + db.Clinics.Find(appointments.ClinicId).Name;
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

                // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
                var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
               .Select(bookedSlot => bookedSlot.SlotId)
               .ToList();
                //var availableTimeSlots = db.TimeSlots.Where(timeSlot => !bookedSlotsForStaffOnDate.Contains(timeSlot.SlotId)).ToList();

                // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name");


                return View(appointments);
            }
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
            BookedSlot bookedSlot = db.BookedSlots.Find(id);
            db.BookedSlots.Remove(bookedSlot);
            db.Appointments.Remove(appointments);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Select()
        {
            var appointments = TempData["AppointmentData"] as Appointments;
            if (appointments== null)
            {
                return RedirectToAction("Create");
            }
            else
            {
                ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName", appointments.PatientUserId);
                ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);

                var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
                ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName");
                return View(appointments);
            }

           //var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
            //ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName");
            //return View(appointments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult Select(Appointments model)
        {
            ModelState.Clear();
            TempData["AppointmentData"] = model; // Store the model data in TempData
            return RedirectToAction("SelectTime");
        }

        public ActionResult SelectTime()
        {
            
            var appointments = TempData["AppointmentData"] as Appointments;
            if (appointments == null)
            {
                return RedirectToAction("Select");
            }
            //var staffIdsInClinic = db.WorkClinic.Where(workClinic => workClinic.ClinicId == appointments.ClinicId).Select(workClinic => workClinic.StaffId).ToList();
            //ViewBag.StaffUserId = new SelectList(db.AspNetUsers.Where(a => staffIdsInClinic.Contains(a.Id)), "Id", "FullName");

            var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
                .Select(bookedSlot => bookedSlot.SlotId)
                .ToList();
            //var availableTimeSlots = db.TimeSlots.Where(timeSlot => !bookedSlotsForStaffOnDate.Contains(timeSlot.SlotId)).ToList();

            // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
            ViewBag.PatientUserId = new SelectList(db.AspNetUsers, "Id", "FullName", appointments.PatientUserId);
            ViewBag.StaffUserId = new SelectList(db.AspNetUsers, "Id", "FullName", appointments.StaffUserId);
            ViewBag.ClinicId = new SelectList(db.Clinics, "Id", "Name", appointments.ClinicId);
            ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name");

            return View(appointments); ;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Patient")]
        public ActionResult SelectTime(Appointments appointments)
        {
            appointments.Id = Guid.NewGuid().ToString();
            ModelState.Clear();
            TryValidateModel(appointments);
            if (ModelState.IsValid)
            {
                try
                {
                    BookedSlot bookedSlot = new BookedSlot();
                    bookedSlot.BookingId = appointments.Id;
                    bookedSlot.SlotId = appointments.TimeSlotId;
                    bookedSlot.StaffUserId = appointments.StaffUserId;
                    bookedSlot.Date = appointments.Date;
                    db.BookedSlots.Add(bookedSlot);

                    db.Appointments.Add(appointments);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            var errorMessage = $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
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

                    var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date).Select(bookedSlot => bookedSlot.SlotId).ToList();
                    ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name");

                    return View(appointments); // Return to the same view to display validation errors
                }

                const String API_KEY = "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";
                try
                {
                    var client = new SendGridClient(API_KEY);
                    var from = new EmailAddress("zhuyanqi001215@gmail.com", "FIT5032 Example Email User");
                    var toPatient = new EmailAddress(db.AspNetUsers.Find(appointments.PatientUserId).Email, db.AspNetUsers.Find(appointments.PatientUserId).FullName);
                    var toStaff = new EmailAddress(db.AspNetUsers.Find(appointments.StaffUserId).Email, db.AspNetUsers.Find(appointments.StaffUserId).FullName);
                    var plainTextContent = "Your Appointment book successfully! \nHere is your appointment information: Appointment " + appointments.Date + " " + appointments.TimeSlot + " at " + db.Clinics.Find(appointments.ClinicId).Name;
                    var htmlContent = "<p>" + plainTextContent + "</p>";
                    var msg = MailHelper.CreateSingleEmail(from, toPatient, "Your Appointment book successfully", plainTextContent, htmlContent);
                    var response = client.SendEmailAsync(msg).Result; // Send the email to Patient
                    msg = MailHelper.CreateSingleEmail(from, toStaff, "Your have a new Appointment", plainTextContent, htmlContent);
                }
                catch
                {
                    return View();
                }
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

                // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
                var bookedSlotsForStaffOnDate = db.BookedSlots.Where(bookedSlot => bookedSlot.StaffUserId == appointments.StaffUserId && bookedSlot.Date == appointments.Date)
               .Select(bookedSlot => bookedSlot.SlotId)
               .ToList();
                //var availableTimeSlots = db.TimeSlots.Where(timeSlot => !bookedSlotsForStaffOnDate.Contains(timeSlot.SlotId)).ToList();

                // Ensure ViewBag.TimeSlotId is of type List<SelectListItem>
                ViewBag.TimeSlotId = new SelectList(db.TimeSlots.Where(a => !bookedSlotsForStaffOnDate.Contains(a.SlotId)), "SlotId", "Name");


                return View(appointments);
            }
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
