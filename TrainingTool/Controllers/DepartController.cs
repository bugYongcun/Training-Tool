using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingTool.Models;
using TrainingTool.Repository;

namespace TrainingTool.Controllers
{
    public class DepartController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();

        //
        // GET: /Depart/

        public ActionResult Index()
        {
            return View(db.Departs.ToList());
        }

        //
        // GET: /Depart/Details/5

        public ActionResult Details(int id = 0)
        {
            Depart depart = db.Departs.Find(id);
            if (depart == null)
            {
                return HttpNotFound();
            }
            return View(depart);
        }

        //
        // GET: /Depart/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Depart/Create

        [HttpPost]
        public ActionResult Create(Depart depart)
        {
            if (ModelState.IsValid)
            {
                db.Departs.Add(depart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(depart);
        }

        //
        // GET: /Depart/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Depart depart = db.Departs.Find(id);
            if (depart == null)
            {
                return HttpNotFound();
            }
            return View(depart);
        }

        //
        // POST: /Depart/Edit/5

        [HttpPost]
        public ActionResult Edit(Depart depart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(depart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(depart);
        }

        //
        // GET: /Depart/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Depart depart = db.Departs.Find(id);
            if (depart == null)
            {
                return HttpNotFound();
            }
            return View(depart);
        }

        //
        // POST: /Depart/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Depart depart = db.Departs.Find(id);
            db.Departs.Remove(depart);
            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}