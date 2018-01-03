﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Identity_CodeAlong.Models;

namespace Identity_CodeAlong.Controllers
{
    public class GymClassesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: GymClasses
        public ActionResult Index()
        {
            List<GymClassIndexModel> model = new List<GymClassIndexModel>();
            foreach (GymClass gc in db.GymClasses.Where(x => x.StartTime >= DateTime.Now).ToList())
            {
                GymClassIndexModel index = new GymClassIndexModel();
                index.Id = gc.Id;
                index.Name = gc.Name;
                index.StartTime = gc.StartTime;
                index.Duration = gc.Duration;
                index.Description = gc.Description;

                if (gc.AttendingMembers.FirstOrDefault(x => x.UserName == User.Identity.Name) == null)
                {
                    index.Attending = "Attend";
                }
                else
                {
                    index.Attending = "Cancel";
                }
                model.Add(index);
            }
            return View(model);
        }

        //  this view is for the user to see all the classes that he had booked
        [Authorize]
        public ActionResult BookedClasses ()
        {
            List<GymClassIndexModel> model = new List<GymClassIndexModel>();
            foreach (GymClass gc in db.GymClasses.Where(x => x.StartTime >= DateTime.Now
            && x.AttendingMembers.FirstOrDefault(y => y.UserName == User.Identity.Name) != null
            ).ToList())
            {
                GymClassIndexModel index = new GymClassIndexModel();
                index.Id = gc.Id;
                index.Name = gc.Name;
                index.StartTime = gc.StartTime;
                index.Duration = gc.Duration;
                index.Description = gc.Description;
                index.Attending = "Cancel";
               
                model.Add(index);
            }
            return View(model);
        }

         //
         [Authorize]
         public ActionResult PersonalHistory()
        {
            List<GymClassIndexModel> model = new List<GymClassIndexModel>();
            foreach (GymClass gc in db.GymClasses.Where(x => x.StartTime < DateTime.Now
            && x.AttendingMembers.FirstOrDefault(y => y.UserName == User.Identity.Name) != null
            ).ToList())
            {
                GymClassIndexModel index = new GymClassIndexModel();
                index.Id = gc.Id;
                index.Name = gc.Name;
                index.StartTime = gc.StartTime;
                index.Duration = gc.Duration;
                index.Description = gc.Description;
                 

                model.Add(index);
            }
            return View(model);
        }


        // GET: GymClasses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize(Roles ="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Create([Bind(Include = "Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.GymClasses.Add(gymClass);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gymClass).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles ="Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GymClass gymClass = db.GymClasses.Find(id);
            if (gymClass == null)
            {
                return HttpNotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles ="Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            GymClass gymClass = db.GymClasses.Find(id);
            db.GymClasses.Remove(gymClass);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult BookingToggle(int id)
        {
            GymClass currentClass = db.GymClasses.FirstOrDefault(x => x.Id == id);
            ApplicationUser currentUser = db.Users.FirstOrDefault(x=>x.UserName == User.Identity.Name);

            if(currentClass.AttendingMembers.Contains(currentUser))
            {
                currentClass.AttendingMembers.Remove(currentUser);
                db.SaveChanges();
            }
            else
            {
                currentClass.AttendingMembers.Add(currentUser);
                db.SaveChanges();
            }
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
