using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingTool.Models;
using TrainingTool.Repository;

namespace TrainingTool.Controllers
{
    public class ChapterController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();

        //
        // GET: /Chapter/

        public ActionResult Index()
        {
            return View(db.chapters.ToList());
        }
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
        // GET: /Chapter/Details/5

        public ActionResult Details(DateTime lastEnterTime, string userId, int id = 0, int courseId = 0, int lastID = 0)
        {
            DateTime startTime = DateTime.Now;
            ViewData["startTime"] = startTime;
            if (lastID != 0)
            {
                int saveCount = db.saveRecords.Where(r => r.courseID == courseId && r.readChapter == lastID && r.userID == userId).ToList().Count;
                int taskCount = db.taskRecords.Where(r => r.courseID == courseId && r.readChapter == lastID && r.userID == userId).ToList().Count;
                DateTime nowTime = DateTime.Now;
                TimeSpan duration = nowTime.Subtract(lastEnterTime);
                double learningMinutes = duration.TotalMinutes;
                if (saveCount > 0)
                {
                    saveRecord record = db.saveRecords.FirstOrDefault(r => r.courseID == courseId && r.readChapter == lastID && r.userID == userId);
                    record.learnMinutes = record.learnMinutes + learningMinutes;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    saveRecord newRecord = new saveRecord();
                    newRecord.courseID = courseId;
                    newRecord.learnMinutes = learningMinutes;
                    newRecord.readChapter = lastID;
                    newRecord.saveTime = DateTime.Now;
                    newRecord.userID = userId;
                    db.saveRecords.Add(newRecord);
                    db.SaveChanges();
                }
                if (taskCount > 0)
                {
                    taskRecord record = db.taskRecords.FirstOrDefault(r => r.courseID == courseId && r.readChapter == lastID && r.userID == userId);
                    record.learnMinutes = record.learnMinutes + learningMinutes;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    taskRecord newRecord = new taskRecord();
                    newRecord.courseID = courseId;
                    newRecord.learnMinutes = learningMinutes;
                    newRecord.readChapter = lastID;
                    newRecord.taskTime = DateTime.Now;
                    newRecord.userID = userId;

                    db.taskRecords.Add(newRecord);
                    db.SaveChanges();
                }
            }
            Chapter chapter = db.chapters.Find(id);
            HttpCookie _cookie = Request.Cookies["User"];
            string userID = _cookie["EmployeeId"];
            if (hasTrainee(userID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
           
            string courseName=db.Courses.Find(chapter.courseID).Name;
            ViewData["courseName"] = courseName;
            if (chapter.chapterID == 1)
            {
                ViewData["lastChapter"] = "";
                ViewData["lastChapterId"] = 0;
            }
            else
            {
               Chapter lastChapter=db.chapters.FirstOrDefault(c => c.courseID == chapter.courseID && c.chapterID == (chapter.chapterID - 1));
                ViewData["lastChapter"] = lastChapter.Name;
                ViewData["lastChapterId"] = lastChapter.ID;
            }
            if (chapter.chapterID == db.chapters.Where(c => c.courseID == chapter.courseID).Count())
            {
                ViewData["followingChapter"] = "";
                ViewData["followingChapterId"] =0;
            }
            else
            {
                Chapter followingChapter=db.chapters.FirstOrDefault(c => c.courseID == chapter.courseID && c.chapterID == (chapter.chapterID +1));
                ViewData["followingChapter"] = followingChapter.Name;
                ViewData["followingChapterId"] = followingChapter.ID;
            }
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // GET: /Chapter/Create

        public ActionResult Create(int courseID)
        {
            ViewData["nowCourse"] = courseID;
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
            return View();
        }

        //
        // POST: /Chapter/Create

        [HttpPost]
        public ActionResult Create(Chapter chapter)
        {
            string courseId = Request.Form["Course"];
            chapter.courseID = int.Parse(courseId);
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
            HttpPostedFileBase file1 = Request.Files["contentfile"];
            HttpPostedFileBase file2 = Request.Files["videofile"];
            //string teacher = db.Courses.Find(chapter.courseID).TeacherID;
         //   string lastPath1 = chapter.courseID.ToString() + "/" + Path.GetFileName(file1.FileName);
         //   string lastPath2 = chapter.courseID.ToString() + "/" + Path.GetFileName(file2.FileName);
          //  string lastPath1 = Path.GetFileName(file1.FileName);
          //  string lastPath2 =  Path.GetFileName(file2.FileName);
            int chapterCount= db.chapters.Where(c => c.courseID == chapter.courseID).Count();
            chapter.chapterID= chapterCount > 0 ? (chapterCount + 1) : 1;
           
              
            if (ModelState.IsValid)
            {
                if (file1 != null)
                {
                   // string fileExt = Path.GetExtension(file1.FileName);
                    // 确认接受到的文件的类型
                    //if (fileExt != "pdf") return View(chapter);//报错。。。待续。。。
                   // string filePath1 = Path.Combine(HttpContext.Server.MapPath("~/Courses"), chapter.courseID.ToString(), Path.GetFileName(file1.FileName));
                    string name = Path.GetFileName(file1.FileName);
                    string type = name.Substring(name.LastIndexOf(".") + 1);  
                    string newName=courseId +"~"+chapter.chapterID.ToString()+"content."+type;
                    string filePath1 = Path.Combine(HttpContext.Server.MapPath("~/Courses"), newName);
                    //Directory.GetAccessControl(filePath1);
                    // 检测当前要存储的文件夹是否存在
                /*    if (!Directory.Exists(filePath1))
                    {
                        // 不存在则创建一个
                        Directory.CreateDirectory(filePath1);
                    }*/
                    chapter.contentFile = newName;
                    file1.SaveAs(filePath1);
                }
                if (file2 != null)
                {
                    // string fileExt = Path.GetExtension(file1.FileName);
                    // 确认接受到的文件的类型
                    //if (fileExt != "pdf") return View(chapter);//报错。。。待续。。。
                    // string filePath1 = Path.Combine(HttpContext.Server.MapPath("~/Courses"), chapter.courseID.ToString(), Path.GetFileName(file1.FileName));
                    string name = Path.GetFileName(file2.FileName);
                    string type = name.Substring(name.LastIndexOf(".") + 1);
                    string newName = courseId + "~" + chapter.chapterID.ToString() + "video." + type;
                    string filePath2 = Path.Combine(HttpContext.Server.MapPath("~/Courses"), newName);
                    //Directory.GetAccessControl(filePath1);
                    // 检测当前要存储的文件夹是否存在
                    /*    if (!Directory.Exists(filePath1))
                        {
                            // 不存在则创建一个
                            Directory.CreateDirectory(filePath1);
                        }*/
                    chapter.videoFile = newName;
                    file2.SaveAs(filePath2);
                }
              /*  else if (file2 != null)
                {
                    string filePath2 = Path.Combine(HttpContext.Server.MapPath("~/App_Data"), lastPath2);
                    // 检测当前要存储的文件夹是否存在
                    if (!Directory.Exists(filePath2))
                    {
                        // 不存在则创建一个
                        Directory.CreateDirectory(filePath2);
                    }
                    chapter.videoFile = filePath2;
                    file2.SaveAs(filePath2);
                }*/
                else
                {
                    Response.Write("Wrong!!");
                }
                db.chapters.Add(chapter);
                db.SaveChanges();
               // return RedirectToAction("Index", "Chapter");
                return RedirectToAction("Edit", "Course", new { id = chapter.courseID });
            }

            return View(chapter);
        }

        //
        // GET: /Chapter/Edit/5

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
            Chapter chapter = db.chapters.Find(id);
            string courseName = db.Courses.Find(chapter.courseID).Name;
            ViewData["courseName"] = courseName;
            if (chapter.chapterID == 1)
            {
                ViewData["lastChapter"] = "";
                ViewData["lastChapterId"] = 0;
            }
            else
            {
                Chapter lastChapter = db.chapters.FirstOrDefault(c => c.courseID == chapter.courseID && c.chapterID == (chapter.chapterID - 1));
                ViewData["lastChapter"] = lastChapter.Name;
                ViewData["lastChapterId"] = lastChapter.ID;
            }
            if (chapter.chapterID == db.chapters.Where(c => c.courseID == chapter.courseID).Count())
            {
                ViewData["followingChapter"] = "";
                ViewData["followingChapterId"] = 0;
            }
            else
            {
                Chapter followingChapter = db.chapters.FirstOrDefault(c => c.courseID == chapter.courseID && c.chapterID == (chapter.chapterID + 1));
                ViewData["followingChapter"] = followingChapter.Name;
                ViewData["followingChapterId"] = followingChapter.ID;
            }
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // POST: /Chapter/Edit/5

        [HttpPost]
        public ActionResult Edit(int id=0,string method="post")
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
            Chapter chapter = db.chapters.Find(id);
             
              string oldfile = Path.Combine(HttpContext.Server.MapPath("~/Courses"), chapter.contentFile);
             
              System.IO.File.Delete(oldfile);
              HttpPostedFileBase file = Request.Files["newcontentfile"];
              if (file != null)
              {
                 // string filePath = Path.Combine(HttpContext.Server.MapPath("~/Courses"), Path.GetFileName(file.FileName));
                  file.SaveAs(oldfile);
                  chapter.contentFile = oldfile;
              }
              db.Entry(chapter).State = EntityState.Modified;
              db.SaveChanges();
              string courseName = db.Courses.Find(chapter.courseID).Name;
              ViewData["courseName"] = courseName;
              return RedirectToAction("Edit", new { id = id });
            //return View(chapter);
        }

        //
        // GET: /Chapter/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Chapter chapter = db.chapters.Find(id);
            if (chapter == null)
            {
                return HttpNotFound();
            }
            return View(chapter);
        }

        //
        // POST: /Chapter/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Chapter chapter = db.chapters.Find(id);
            db.chapters.Remove(chapter);
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