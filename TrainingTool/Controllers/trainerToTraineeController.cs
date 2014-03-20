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
    public class trainerToTraineeController : Controller
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
        // GET: /trainerToTrainee/

        public ActionResult ViewTrainee(string trainerID)//之后要修改链接与参数
        {
            //string trainerID = "1463001";
            ViewData["trainer"] = trainerID;
          
           List<TrainerToTrainee> pairs=db.trainerTotrainee.Where(t => t.trainerId == trainerID).ToList();
            List<User> trainees=new List<User>();
           foreach (var p in pairs)
           {
              User trainee= db.User.FirstOrDefault(u => u.EmployeeId == p.traineeId);
              trainees.Add(trainee);
           }
           return View(trainees);
        }
      
     
        public ActionResult GiveTask(string trainerID,string traineeID)
        {
            List<Course> courses = db.Courses.ToList();
            ViewData["trainer"] = trainerID;
            ViewData["trainee"] = traineeID;
            CourseToAssign vmd = new CourseToAssign();
            List<courseItem> checkBoxList = new List<courseItem>();
           List<taskRecord> records= db.taskRecords.Where(t => t.userID == traineeID && t.assignBy.Contains(trainerID) && t.readChapter == -1).ToList();
            List<Course> allcourses = db.Courses.ToList();
            if (records != null)
            {
                foreach (var r in records)
                {
                    Course course = db.Courses.Find(r.courseID);
                    allcourses.Remove(course);
                }
            }
                //Add data in SelectList List
            foreach (var d in allcourses)
                {
                    courseItem chk = new courseItem();
                    chk.Category = d.Type;
                    chk.Selected = false;
                    chk.forDeparts = d.ForDepart;
                    chk.name = d.Name;
                    chk.courseID = d.ID;
                   chk.authorName= db.User.FirstOrDefault(u => u.EmployeeId == d.TeacherID).UserName;
                   chk.authorDepart = db.User.FirstOrDefault(u => u.EmployeeId == d.TeacherID).DepartmentId;
                    checkBoxList.Add(chk);
                }
            vmd.coursesList = checkBoxList;
            return View(vmd);
        }

        //
        // POST: /trainerToTrainee/Create

        [HttpPost]
        public ActionResult GiveTask(string trainerID,string traineeID,CourseToAssign vmd)
        {
            foreach (var item in vmd.coursesList)
            {
                if (item.Selected)
                {
                    int assignTime = db.taskRecords.Where(t => t.courseID == item.courseID && t.userID == traineeID).ToList().Count;
                    if(assignTime>0)
                    {
                        taskRecord task = db.taskRecords.FirstOrDefault(t => t.courseID == item.courseID && t.userID == traineeID && t.readChapter == -1);
                        task.assignBy = task.assignBy + "," + trainerID;
                        db.Entry(task).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                   taskRecord task=new taskRecord();
                   task.assignBy=trainerID;
                    task.courseID=item.courseID;
                    task.learnMinutes=0.00;
                    task.readChapter=-1;
                    task.taskTime=DateTime.Now;
                    task.userID=traineeID;
                    db.taskRecords.Add(task);}
                }
            }
            db.SaveChanges();

            return RedirectToAction("ViewTask", new { trainerID = trainerID, traineeID = traineeID });
        }

        public ActionResult ViewTask(string trainerID, string traineeID)
        {
          
            List<taskRecord> taskCourse = db.taskRecords.Where(r => r.userID == traineeID && r.assignBy.Contains(trainerID) && r.readChapter == -1).ToList();
            ViewData["trainer"] = trainerID;
            ViewData["trainee"] = traineeID;
            //List<taskRecord> tasks = db.taskRecords.Where(r => r.courseID == taskCourse && r.userID == traineeID && r.readChapter > 0).ToList();
            return View(taskCourse);
        }
        public ActionResult TaskDetail(string trainerID, string traineeID, int courseID)
        {
            List<taskRecord> taskCourse = db.taskRecords.Where(r => r.userID == traineeID && r.courseID==courseID&&r.readChapter>0).ToList();
            if (taskCourse == null)
            {
                ViewData["progress"] = 0;
            }
            else
            {
                ViewData["progress"] = 1;
            }
            ViewData["trainer"] = trainerID;
            ViewData["trainee"] = traineeID;
            ViewData["course"] = courseID;
            List<Chapter> chapters = db.chapters.Where(c => c.courseID == courseID).ToList();
            //List<taskRecord> tasks = db.taskRecords.Where(r => r.courseID == taskCourse && r.userID == traineeID && r.readChapter > 0).ToList();
            return View(chapters);
        }

        //
        public ActionResult DeleteTask(string trainerID, string traineeID,int courseID)
        {
            List<Course> courses = db.Courses.ToList();
            ViewData["trainer"] = trainerID;
            ViewData["trainee"] = traineeID;
           taskRecord record=db.taskRecords.FirstOrDefault(t => t.userID == traineeID && t.assignBy.Contains(trainerID) && t.readChapter == -1);
           if (record.assignBy.Contains(","))
           {
              
               string[] ids = record.assignBy.Split(',');
               record.assignBy = "";
               int splitCount = ids.Length - 2;
               for (int i = 0; i < ids.Length; i++)
               {
                   if (ids[i] != trainerID)
                   {
                       record.assignBy = record.assignBy + ids[i];
                       if (splitCount>0)
                       {
                           record.assignBy = record.assignBy + ",";
                           splitCount--;
                       }
                   }
                  
               }
               db.Entry(record).State = EntityState.Modified;
               db.SaveChanges();
           }
           else
           {
               db.taskRecords.Remove(record);
               db.SaveChanges();
           }

           return RedirectToAction("ViewTask", new { trainerID=trainerID, traineeID=traineeID});
        }

        //
        // POST: /trainerToTrainee/Create


        // GET: /trainerToTrainee/Edit/5

        public ActionResult Edit(int id = 0)
        {
            TrainerToTrainee trainertotrainee = db.trainerTotrainee.Find(id);
            if (trainertotrainee == null)
            {
                return HttpNotFound();
            }
            return View(trainertotrainee);
        }

        //
        // POST: /trainerToTrainee/Edit/5

        [HttpPost]
        public ActionResult Edit(TrainerToTrainee trainertotrainee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(trainertotrainee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(trainertotrainee);
        }

        //
        // GET: /trainerToTrainee/Delete/5

        public ActionResult Delete(int id = 0)
        {
            TrainerToTrainee trainertotrainee = db.trainerTotrainee.Find(id);
            if (trainertotrainee == null)
            {
                return HttpNotFound();
            }
            return View(trainertotrainee);
        }

        //
        // POST: /trainerToTrainee/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            TrainerToTrainee trainertotrainee = db.trainerTotrainee.Find(id);
            db.trainerTotrainee.Remove(trainertotrainee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
    public class CourseToAssign
    {
        public List<courseItem> coursesList { get; set; }
    }

    public class courseItem
    {
        public string name { get; set; }
        public string Category { get; set; }  
        public string authorName { get; set; }
        public string authorDepart { get; set; }
        public string forDeparts { get; set; }
        public int courseID { get; set; }
        public bool Selected { get; set; }
    }
}