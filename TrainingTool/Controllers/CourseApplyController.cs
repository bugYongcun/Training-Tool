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
    public class CourseApplyController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();
        public bool hasTrainee(string userId)
        {
            int traineeCount = db.trainerTotrainee.Where(t => t.trainerId == userId).ToList().Count;
            if (traineeCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //
        // GET: /CourseApply/

        public ActionResult Index()
        {
            return View(db.CourseApplies.ToList());
        }

        //
        // GET: /CourseApply/Details/5

        public ActionResult Details(int id = 0)
        {
            CourseApply courseapply = db.CourseApplies.Find(id);
            if (courseapply == null)
            {
                return HttpNotFound();
            }
            return View(courseapply);
        }

        //
        // GET: /CourseApply/Create

        public ActionResult Create()
        {
            HttpCookie _cookie = Request.Cookies["User"];
            string userId = _cookie["EmployeeId"];
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            List<Depart> departList = db.Departs.ToList();
            List<string> departNames = new List<string>();

            foreach (Depart d in departList)
            {
                departNames.Add(d.Name);
            }

            ViewData["list"] = departNames;
            int count1 = db.CourseTypes.Where(t => t.typeID.StartsWith("1")).Count();
            int count2 = db.CourseTypes.Where(t => t.typeID.StartsWith("2")).Count();
            int count3 = db.CourseTypes.Where(t => t.typeID.StartsWith("3")).Count();

            ViewData["count1"] = count1;
            ViewData["count2"] = count2;
            ViewData["count3"] = count3;
            /*  List<string> departNameList = new List<string>();
              List<Depart> departList = db.Departs.ToList();
              foreach (Depart d in departList)
              {
                  departNameList.Add(d.Name);
              }
           
             ViewBag.DepList = departNameList;*/
            //MultiSelectList dL = new MultiSelectList(departList);
            // ViewData["departs"] = departNameList;
            return View();
        }

        [HttpPost]
        public ActionResult Create(CourseApply courseapply)
        {
           
            if (ModelState.IsValid)
            {
                string forDeparts = Request.Form["ForDepart"];
                courseapply.ForDepart = forDeparts;
                courseapply.ApplyDate = DateTime.Now;
                HttpCookie _cookie = Request.Cookies["User"];
                string teacherID = _cookie["EmployeeId"];
                if (hasTrainee(teacherID))
                {
                    ViewData["hasTrainee"] = 1;
                }
                else
                {
                    ViewData["hasTrainee"] = 0;
                }
                courseapply.TeacherID = teacherID;

                db.CourseApplies.Add(courseapply);
                db.SaveChanges();
                return RedirectToAction("teachersIndex", "Course", new { lastTime = DateTime.Now, userId = teacherID });
            }

            return RedirectToAction("Index", "Course");   
        }

        //
        // GET: /CourseApply/Edit/5

        public ActionResult Edit(int id = 0)
        {
            HttpCookie _cookie = Request.Cookies["User"];
            string userId = _cookie["EmployeeId"];
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            CourseApply courseapply = db.CourseApplies.Find(id);
            if (courseapply == null)
            {
                return HttpNotFound();
            }
            return View(courseapply);
        }

        //
        // POST: /CourseApply/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseApply courseapply)
        {
            HttpCookie _cookie = Request.Cookies["User"];
            string userId = _cookie["EmployeeId"];
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            if (ModelState.IsValid)
            {
                db.Entry(courseapply).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courseapply);
        }

        //
        // GET: /CourseApply/Delete/5

        public ActionResult Delete(int id = 0)
        {
            HttpCookie _cookie = Request.Cookies["User"];
            string userId = _cookie["EmployeeId"];
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            CourseApply courseapply = db.CourseApplies.Find(id);
            if (courseapply == null)
            {
                return HttpNotFound();
            }
            return View(courseapply);
        }

        //
        // POST: /CourseApply/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseApply courseapply = db.CourseApplies.Find(id);
            db.CourseApplies.Remove(courseapply);
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