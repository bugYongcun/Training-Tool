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
using PagedList;
using PagedList.Mvc;
using json.Models;

namespace TrainingTool.Controllers
{
    public class CourseController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();
        public bool hasTrainee(string userId)
        {
           int traineeCount= db.trainerTotrainee.Where(t => t.trainerId == userId).ToList().Count;
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
        // GET: /Course/
        public ActionResult Index(DateTime lastTime, string userId, int courseId = 0, int lastChapterID = 0, int page = 1)
        {

            if (lastChapterID != 0)
            {
                recordTime(lastTime, userId, courseId, lastChapterID);
            }
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
            departNames.Add("All");
            foreach (Depart d in departList)
            {
                departNames.Add(d.Name);
            }

            ViewData["list"] = departNames;
            string Department = Request.Form["Department"];
            string Keyword = Request.Form["Keyword"];
            List<Course> courselist = new List<Course>();

            if (!String.IsNullOrEmpty(Keyword))
            {
                courselist = db.Courses.Where(m => (m.Name.ToUpper().Contains(Keyword.ToUpper())
                    || m.Introduction.ToUpper().Contains(Keyword.ToUpper()))).ToList();

            }
            else
            {
                courselist = db.Courses.ToList();
            }
            //List<User> userlist = new List<User>();
            List<CourseList> courseitems = new List<CourseList>();

            foreach (Course c in courselist)
            {
                User trainer = new User();
                trainer.Id = db.User.FirstOrDefault(u => u.EmployeeId == c.TeacherID).Id;
                trainer.DepartmentId = db.User.FirstOrDefault(u => u.EmployeeId == c.TeacherID).DepartmentId;
                trainer.EmployeeId = db.User.FirstOrDefault(u => u.EmployeeId == c.TeacherID).EmployeeId;
                trainer.UserName = db.User.FirstOrDefault(u => u.EmployeeId == c.TeacherID).UserName;
                CourseType theType = new CourseType();
                theType.ID = db.CourseTypes.FirstOrDefault(t => t.Name == c.Type).ID;
                theType.Name = db.CourseTypes.FirstOrDefault(t => t.Name == c.Type).Name;
                theType.typeID = db.CourseTypes.FirstOrDefault(t => t.Name == c.Type).typeID;
                Course temp = new Course();
                temp.ForDepart = c.ForDepart;
                temp.ID = c.ID;
                temp.Name = c.Name;
                temp.Type = c.Type;
                temp.Introduction = c.Introduction;
                if (String.IsNullOrEmpty(Department) || (Department == "All"))
                {//全都加上
                    CourseList courseItem = new CourseList();
                    courseItem.course = temp;
                    courseItem.type = theType;
                    courseItem.user = trainer;
                    courseitems.Add(courseItem);
                }
                else
                {
                    if (temp.ForDepart.Contains(Department))
                    {
                        CourseList courseItem = new CourseList();
                        courseItem.course = temp;
                        courseItem.type = theType;
                        courseItem.user = trainer;
                        courseitems.Add(courseItem);
                    }

                }

            }
            ViewBag.CurrentKeyword = String.IsNullOrEmpty(Keyword) ? "" : Keyword;
            ViewBag.currentDepart = String.IsNullOrEmpty(Department) ? "All" : Department;
           
            ViewData["Courses"] = courseitems;
           string userDepart= db.User.FirstOrDefault(u=>u.EmployeeId==userId).DepartmentId;
           List<Course> forCourseList = db.Courses.Where(c => c.ForDepart.Contains(userDepart)).ToList();
           List<int> forList = new List<int>();
           foreach (Course r in forCourseList)
           {
               forList.Add(r.ID);
           }
           ViewData["forCourses"] = forList;
            List<taskRecord> taskList = db.taskRecords.Where(r => r.userID == userId&&r.readChapter==-1).ToList();
            List<int> tasklist = new List<int>();
            foreach (taskRecord r in taskList)
            {
                tasklist.Add(r.courseID);
            }
            ViewData["taskCourses"] = tasklist;

            List<saveRecord> saveRecordList = db.saveRecords.Where(s => s.userID == userId && s.readChapter == -1).ToList();
            List<int> savedcourselist = new List<int>();
            foreach (saveRecord s in saveRecordList)
            {
               // Course course = db.Courses.FirstOrDefault(c => c.ID == s.courseID);
                savedcourselist.Add(s.courseID);
            }
            ViewData["saveCourses"] = savedcourselist;

            List<learningRecord> learningList = db.learningRecords.Where(r => r.userID == userId).ToList();
            List<int> learningcourselist = new List<int>();
            foreach (learningRecord r in learningList)
            {
                learningcourselist.Add(r.courseID);
            }
            ViewData["learningCourses"] = learningcourselist;

            List<learningApply> requestList = db.learningApplys.Where(r =>  r.userID == userId).ToList();
            List<int> requestcourselist = new List<int>();
            foreach (learningApply r in requestList)
            {
                requestcourselist.Add(r.courseID);
            }
            ViewData["requestCourses"] = requestcourselist;
           
           
            int currentPage = (page - 1 > 0) ? page : 1;
            int pageSize = 1;
            int totalCount = 0;
            var courses = GetCourse(currentPage, pageSize, ref totalCount);
            var coursesAsIPagedList = new StaticPagedList<Course>(courses, currentPage, pageSize, totalCount);

            return View(coursesAsIPagedList);
        }
        public ActionResult studentIndex(DateTime lastTime, string userId, int courseId = 0, int lastChapterID = 0, int page = 1)
        {

            if (lastChapterID != 0)
            {
                recordTime(lastTime, userId, courseId, lastChapterID);
            }
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            List<taskRecord> taskList = db.taskRecords.Where(r => r.userID == userId && r.readChapter == -1).ToList();
            List<Course> tasklist = new List<Course>();
           // List<int> containedTask = new List<int>();
            foreach (taskRecord r in taskList)
            {
               //  if(!containedTask.Contains(r.courseID))
              // {
                tasklist.Add(db.Courses.FirstOrDefault(c => c.ID == r.courseID));
               // containedTask.Add(r.courseID);
             //  }
            }
            ViewData["taskCourses"] = tasklist;

            List<saveRecord> saveRecordList = db.saveRecords.Where(s => s.userID == userId && s.readChapter == -1).ToList();
            List<Course> savedcourselist = new List<Course>();
           // List<int> containedCourse = new List<int>();
           
            foreach (saveRecord s in saveRecordList)
            {
               //if(!containedCourse.Contains(s.courseID))
               //{
                savedcourselist.Add(db.Courses.FirstOrDefault(c => c.ID == s.courseID));
                //containedCourse.Add(s.courseID);
             //  }
            }
            ViewData["saveCourses"] = savedcourselist;
            // ViewData["saveCourses"] = db.Courses.ToList();
            List<learningApply> requestList = db.learningApplys.Where(r => r.userID == userId).ToList();
            List<Course> requestcourselist = new List<Course>();
            foreach (learningApply r in requestList)
            {
                requestcourselist.Add(db.Courses.FirstOrDefault(c => c.ID == r.courseID));
            }
            ViewData["requestCourses"] = requestcourselist;

            List<learningRecord> learningList = db.learningRecords.Where(r => r.userID == userId).ToList();
            List<Course> learningcourselist = new List<Course>();
            foreach (learningRecord r in learningList)
            {
                learningcourselist.Add(db.Courses.FirstOrDefault(c => c.ID == r.courseID));
            }
            ViewData["learningCourses"] = learningcourselist;

            int currentPage = (page - 1 > 0) ? page : 1;
            int pageSize = 1;
            int totalCount = 0;
            var courses = GetCourse(currentPage, pageSize, ref totalCount);
            var coursesAsIPagedList = new StaticPagedList<Course>(courses, currentPage, pageSize, totalCount);

            return View(coursesAsIPagedList);
        }
        public ActionResult teachersIndex(DateTime lastTime,string userId, int courseId = 0, int lastChapterID = 0,int page = 1)
        {
           
            if (lastChapterID != 0)
            {
                recordTime(lastTime,userId,courseId ,lastChapterID);
            }
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            List<Course> passedcourselist = db.Courses.Where(s => s.TeacherID == userId).ToList();
            List<CourseApply> checkingcourselist = db.CourseApplies.Where(s => s.TeacherID == userId).ToList();
           
            ViewData["passedCourses"] = passedcourselist;
            ViewData["checkingCourses"] = checkingcourselist;

            int currentPage = (page - 1 > 0) ? page : 1;
            int pageSize = 1;
            int totalCount = 0;
            var courses = GetCourse(currentPage, pageSize, ref totalCount);
            var coursesAsIPagedList = new StaticPagedList<Course>(courses, currentPage, pageSize, totalCount);

            return View(coursesAsIPagedList);
        }
     
        public List<Course> GetCourse(int pageIndex, int pageSize, ref int totalCount)
        {
            var courses = (from p in db.Courses
                           orderby p.ID descending
                           select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = db.Courses.Count();
            return courses.ToList();
        }
        public void recordTime(DateTime lastTime,string userId, int courseId = 0, int lastChapterID = 0)
        {

            int saveCount = db.saveRecords.Where(r => r.courseID == courseId && r.readChapter == lastChapterID&&r.userID==userId).ToList().Count;
            int taskCount = db.taskRecords.Where(r => r.courseID == courseId && r.readChapter == lastChapterID && r.userID == userId).ToList().Count;
            DateTime nowTime = DateTime.Now;
            TimeSpan duration = nowTime.Subtract(lastTime);
            double learningMinutes = duration.TotalMinutes;
            if (saveCount > 0)
            {
                saveRecord record = db.saveRecords.FirstOrDefault(r => r.courseID == courseId && r.readChapter == lastChapterID && r.userID == userId);
                record.learnMinutes = record.learnMinutes + learningMinutes;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                saveRecord newRecord = new saveRecord();
                newRecord.courseID = courseId;
                newRecord.learnMinutes = learningMinutes;
                newRecord.readChapter = lastChapterID;
                newRecord.saveTime = DateTime.Now;
                newRecord.userID = userId;
                db.saveRecords.Add(newRecord);
                db.SaveChanges();
            }
            if (taskCount > 0)
            {
                taskRecord record = db.taskRecords.FirstOrDefault(r => r.courseID == courseId && r.readChapter == lastChapterID && r.userID == userId);
                record.learnMinutes = record.learnMinutes + learningMinutes;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                taskRecord newRecord = new taskRecord();
                newRecord.courseID = courseId;
                newRecord.learnMinutes = learningMinutes;
                newRecord.readChapter = lastChapterID;
                newRecord.taskTime = DateTime.Now;
                newRecord.userID = userId;
                db.taskRecords.Add(newRecord);
                db.SaveChanges();
            }
        }
        // GET: /Course/Details/5
        #region Details
        public ActionResult Details(DateTime lastTime,string userId, int courseId = 0, int lastChapterID = 0,int page = 1,int id = 0)
        {
           
            if (lastChapterID != 0)
            {
                recordTime(lastTime,userId,courseId,lastChapterID);
            }
            if (hasTrainee(userId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            Course course = db.Courses.Find(id);
            User trainer=db.User.FirstOrDefault(u => u.EmployeeId == course.TeacherID);
            ViewData["teacherName"] = trainer.UserName;
            ViewData["teacherDepart"] = trainer.DepartmentId;

            List<Chapter> chapters=db.chapters.Where(c=>c.courseID==id).OrderBy(c=>c.chapterID).ToList();
            ViewData["chapters"] = chapters;
           List<saveRecord> saverecord= db.saveRecords.Where(s => s.courseID== id).ToList();
           List<string> saveUsers = new List<string>();
           foreach (var s in saverecord)
           {
               saveUsers.Add(s.userID);
           }
           ViewData["saveusers"] = saveUsers;

           List<taskRecord> taskrecord = db.taskRecords.Where(s => s.courseID == id).ToList();
           List<string> taskUsers = new List<string>();
           foreach (var s in taskrecord)
           {
               taskUsers.Add(s.userID);
           }
           ViewData["taskusers"] = saveUsers;
           List<userAnswer> userAnswers = db.userAnswers.Where(a => a.courseId == id).ToList();
           List<string> testedUsers = new List<string>();
           foreach (var s in userAnswers)
           {
               testedUsers.Add(s.userId);
           }
           ViewData["testedusers"] = testedUsers;

           int testCount = db.testLists.Where(l => l.courseID == id).ToList().Count;
           ViewData["testCount"] = testCount;
          int count= db.learningRecords.Where(r => r.courseID == courseId && r.userID == userId).ToList().Count;
          if (count > 0)
          {
              learningRecord record = db.learningRecords.FirstOrDefault(r => r.courseID == courseId && r.userID == userId);
              record.lastTime = DateTime.Now;
              db.Entry(record).State = EntityState.Modified;
              db.SaveChanges();
          }
          else
          {
              learningRecord newRecord = new learningRecord();
              newRecord.userID = userId;
              newRecord.courseID = courseId;
              newRecord.lastTime = DateTime.Now;
              db.learningRecords.Add(newRecord);
              db.SaveChanges();
          }
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }
          [HttpPost]
        public ActionResult Details()
        {
           double mark=double.Parse( Request.Form["mark"]);
           int id = int.Parse(Request.Form["id"]);
            Course course = db.Courses.Find(id);
            course.markCount++;
            if (course.markCount == 1)
            {
                course.Mark = mark;
            }
            else
            {
                course.Mark = (course.Mark + mark) / course.markCount;
            }
            db.Entry(course).State = EntityState.Modified;
            db.SaveChanges();
            User trainer = db.User.FirstOrDefault(u => u.EmployeeId == course.TeacherID);
            ViewData["teacherName"] = trainer.UserName;
            ViewData["teacherDepart"] = trainer.DepartmentId;

            List<Chapter> chapters = db.chapters.Where(c => c.courseID == id).OrderBy(c => c.chapterID).ToList();
            ViewData["chapters"] = chapters;

            List<saveRecord> saverecord = db.saveRecords.Where(s => s.courseID == id).ToList();
            List<string> saveUsers = new List<string>();
            foreach (var s in saverecord)
            {
                saveUsers.Add(s.userID);
            }
            ViewData["users"] = saveUsers;
            List<userAnswer> userAnswers = db.userAnswers.Where(a => a.courseId == id).ToList();
            List<string> testedUsers = new List<string>();
            foreach (var s in userAnswers)
            {
                testedUsers.Add(s.userId);
            }
            ViewData["testedusers"] = testedUsers;

            int testCount = db.testLists.Where(l => l.courseID == id).ToList().Count;
            ViewData["testCount"] = testCount;
            if (course == null)
            {
                return HttpNotFound();
            }
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
            return View(course);
        }
        #endregion
          public JsonResult getLists()
        {
            var list1 = (from a in db.CourseTypes
                         where a.typeID.StartsWith("1")
                         select new
                         {
                             id = a.typeID,
                             name = a.Name
                         }).ToList();
            var list2 = (from a in db.CourseTypes
                         where a.typeID.StartsWith("2")
                         select new
                         {
                             id = a.typeID,
                             name = a.Name
                         }).ToList();
            var list3 = (from a in db.CourseTypes
                         where a.typeID.StartsWith("3")
                         select new
                         {
                             id = a.typeID,
                             name = a.Name
                         }).ToList();
            JsonResult jsonResult = Json(new { ls1 = list1, ls2 = list2, ls3 = list3 }, JsonRequestBehavior.AllowGet);
            return jsonResult;
        }
    

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
         
            return View();
        }

        [HttpPost]
        public ActionResult Create(Course course)
        {
           
            
            if (ModelState.IsValid)
            {
               string forDeparts= Request.Form["ForDepart"];
               course.ForDepart = forDeparts;
                course.setDate = DateTime.Now;
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
                course.TeacherID = teacherID;
                course.Mark = 0.00;
               
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("teachersIndex", new { userId = teacherID });
            }

            return View(course);
        }
        [HttpPost]
        public void AddNewType([ModelBinder(typeof(JsonBinder<CourseType>))]CourseType newType)
        {
            CourseType newCourseType = newType;
            if (newType != null)
            {
                db.CourseTypes.Add(newType);
                db.SaveChanges();
            }
        }
        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Course course = db.Courses.Find(id);
            List<Chapter> chapters = db.chapters.Where(c => c.courseID == id).ToList();
            
            if (hasTrainee(course.TeacherID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            ViewData["contents"] = chapters;
            int testcount = db.testLists.Where(l => l.courseID == id).ToList().Count;
            ViewData["count"] = testcount;
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }
        public ActionResult learningApply(string userId,int id)
        {
            learningApply apply = new Models.learningApply();
            apply.courseID = id;
            apply.requestTime = DateTime.Now;
            apply.userID = userId;
            db.learningApplys.Add(apply);
            db.SaveChanges();


            //return View();
            return RedirectToAction("Index", "Course", new { lastTime = DateTime.Now, userId = userId });
        }
      
        public ActionResult Save(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
           
            saveRecord saverecord = new saveRecord();
            saverecord.courseID = id;
            saverecord.saveTime = DateTime.Now;
            saverecord.readChapter = -1;
            HttpCookie _cookie = Request.Cookies["User"];
             string nowUserId = _cookie["EmployeeId"];
             if (hasTrainee(nowUserId))
             {
                 ViewData["hasTrainee"] = 1;
             }
             else
             {
                 ViewData["hasTrainee"] = 0;
             }
           // string userID = "1463002";
             saverecord.userID = nowUserId;

            db.saveRecords.Add(saverecord);
            db.SaveChanges();

           
            //return View();
            return RedirectToAction("Details", "Course", new { lastTime=DateTime.Now,id = id });
        }
        public ActionResult unSave(int id = 0)
        {
           
            HttpCookie _cookie = Request.Cookies["User"];
            string nowUserId = _cookie["EmployeeId"];
            if (hasTrainee(nowUserId))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
    
            db.saveRecords.Remove(db.saveRecords.FirstOrDefault(s => (s.userID == nowUserId && s.courseID == id&&s.readChapter==-1)));
            db.SaveChanges();


            //return View();
            return RedirectToAction("Details", "Course", new { lastTime = DateTime.Now, id = id });
        }
      
        // GET: /Course/Delete/5
        public ActionResult Test(string userID, int id = 0)
        {
            if (hasTrainee(userID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            int listCount = db.testLists.Where(m => m.courseID == id).Count();
            string tid1 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test1ID;
            string tid2 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test2ID;
            string tid3 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test3ID;
            string tid4 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test4ID;
            string tid5 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test5ID;
            string tid6 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test6ID;
            string tid7 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test7ID;
            string tid8 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test8ID;
            string tid9 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test9ID;
            string tid10 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test10ID;
            Test t1 = db.tests.FirstOrDefault(t => t.testId == tid1);
            Test t2 = db.tests.FirstOrDefault(t => t.testId == tid2);
            Test t3 = db.tests.FirstOrDefault(t => t.testId == tid3);
            Test t4 = db.tests.FirstOrDefault(t => t.testId == tid4);
            Test t5 = db.tests.FirstOrDefault(t => t.testId == tid5);
            Test t6 = db.tests.FirstOrDefault(t => t.testId == tid6);
            Test t7 = db.tests.FirstOrDefault(t => t.testId == tid7);
            Test t8 = db.tests.FirstOrDefault(t => t.testId == tid8);
            Test t9 = db.tests.FirstOrDefault(t => t.testId == tid9);
            Test t10 = db.tests.FirstOrDefault(t => t.testId == tid10);
            ViewData["test1"] = t1;
            ViewData["test2"] = t2;
            ViewData["test3"] = t3;
            ViewData["test4"] = t4;
            ViewData["test5"] = t5;
            ViewData["test6"] = t6;
            ViewData["test7"] = t7;
            ViewData["test8"] = t8;
            ViewData["test9"] = t9;
            ViewData["test10"] = t10;
            ViewData["type"] = "userAnswer";
            userAnswer answer = new userAnswer();
            answer.courseId = id;
            answer.userId = userID;
            return View(answer);
        }
        [HttpPost]
        public ActionResult Test(userAnswer answer, int id, string userID)
        {
            if (hasTrainee(userID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            if (ModelState.IsValid)
            {
                /* HttpCookie _cookie = Request.Cookies["User"];
                  answer.userId=_cookie["EmployeeId"];
                  answer.courseId =int.Parse(Request.Form["courseID"]);*/
                answer.userId = userID;
                answer.courseId = id;
                db.userAnswers.Add(answer);
                db.SaveChanges();
                int listCount = db.testLists.Where(m => m.courseID == id).Count();
                string tid1 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test1ID;
                string tid2 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test2ID;
                string tid3 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test3ID;
                string tid4 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test4ID;
                string tid5 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test5ID;
                string tid6 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test6ID;
                string tid7 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test7ID;
                string tid8 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test8ID;
                string tid9 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test9ID;
                string tid10 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test10ID;
                Test t1 = db.tests.FirstOrDefault(t => t.testId == tid1);
                Test t2 = db.tests.FirstOrDefault(t => t.testId == tid2);
                Test t3 = db.tests.FirstOrDefault(t => t.testId == tid3);
                Test t4 = db.tests.FirstOrDefault(t => t.testId == tid4);
                Test t5 = db.tests.FirstOrDefault(t => t.testId == tid5);
                Test t6 = db.tests.FirstOrDefault(t => t.testId == tid6);
                Test t7 = db.tests.FirstOrDefault(t => t.testId == tid7);
                Test t8 = db.tests.FirstOrDefault(t => t.testId == tid8);
                Test t9 = db.tests.FirstOrDefault(t => t.testId == tid9);
                Test t10 = db.tests.FirstOrDefault(t => t.testId == tid10);
                ViewData["test1"] = t1;
                ViewData["test2"] = t2;
                ViewData["test3"] = t3;
                ViewData["test4"] = t4;
                ViewData["test5"] = t5;
                ViewData["test6"] = t6;
                ViewData["test7"] = t7;
                ViewData["test8"] = t8;
                ViewData["test9"] = t9;
                ViewData["test10"] = t10;
                ViewData["type"] = "rightAnswer";
                return View(answer);
            }


            return View(answer);
        }
        public ActionResult ViewTest(string userID, int id = 0)
        {
            if (hasTrainee(userID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            int listCount = db.testLists.Where(m => m.courseID == id).Count();
            string tid1 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test1ID;
            string tid2 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test2ID;
            string tid3 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test3ID;
            string tid4 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test4ID;
            string tid5 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test5ID;
            string tid6 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test6ID;
            string tid7 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test7ID;
            string tid8 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test8ID;
            string tid9 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test9ID;
            string tid10 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test10ID;
            Test t1 = db.tests.FirstOrDefault(t => t.testId == tid1);
            Test t2 = db.tests.FirstOrDefault(t => t.testId == tid2);
            Test t3 = db.tests.FirstOrDefault(t => t.testId == tid3);
            Test t4 = db.tests.FirstOrDefault(t => t.testId == tid4);
            Test t5 = db.tests.FirstOrDefault(t => t.testId == tid5);
            Test t6 = db.tests.FirstOrDefault(t => t.testId == tid6);
            Test t7 = db.tests.FirstOrDefault(t => t.testId == tid7);
            Test t8 = db.tests.FirstOrDefault(t => t.testId == tid8);
            Test t9 = db.tests.FirstOrDefault(t => t.testId == tid9);
            Test t10 = db.tests.FirstOrDefault(t => t.testId == tid10);
            ViewData["test1"] = t1;
            ViewData["test2"] = t2;
            ViewData["test3"] = t3;
            ViewData["test4"] = t4;
            ViewData["test5"] = t5;
            ViewData["test6"] = t6;
            ViewData["test7"] = t7;
            ViewData["test8"] = t8;
            ViewData["test9"] = t9;
            ViewData["test10"] = t10;
            ViewData["type"] = "rightAnswer";

            userAnswer answer = db.userAnswers.FirstOrDefault(a => (a.userId == userID && a.courseId == id));
            return View(answer);
        }
        public ActionResult addTest(int id = 0)
        {
          
            Course course = db.Courses.Find(id);
            if (hasTrainee(course.TeacherID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewData["type"] = "create";
            return View(course);
        }

        [HttpPost]
        public ActionResult addTest(Course course)
        {
            if (hasTrainee(course.TeacherID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            if (ModelState.IsValid)
            {
                testList testlist = new testList();
                testlist.courseID = course.ID;
                int editcount = db.testLists.Where(t => t.courseID == course.ID).ToList().Count;
                testlist.editTime = editcount > 0 ? (editcount + 1) : 1;
                Test t1 = new Test();
                t1.content = Request.Form["t1content"];
                t1.chooseA = Request.Form["t1A"];
                t1.chooseB = Request.Form["t1B"];
                t1.chooseC = Request.Form["t1C"];
                t1.chooseD = Request.Form["t1D"];
                t1.answer = Request.Form["t1answer"];
                t1.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~1";
                testlist.test1ID = t1.testId;
                Test t2 = new Test();
                t2.content = Request.Form["t2content"];
                t2.chooseA = Request.Form["t2A"];
                t2.chooseB = Request.Form["t2B"];
                t2.chooseC = Request.Form["t2C"];
                t2.chooseD = Request.Form["t2D"];
                t2.answer = Request.Form["t2answer"];
                t2.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~2";
                testlist.test2ID = t2.testId;
                Test t3 = new Test();
                t3.content = Request.Form["t3content"];
                t3.chooseA = Request.Form["t3A"];
                t3.chooseB = Request.Form["t3B"];
                t3.chooseC = Request.Form["t3C"];
                t3.chooseD = Request.Form["t3D"];
                t3.answer = Request.Form["t3answer"];
                t3.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~3";
                testlist.test3ID = t3.testId;
                Test t4 = new Test();
                t4.content = Request.Form["t4content"];
                t4.chooseA = Request.Form["t4A"];
                t4.chooseB = Request.Form["t4B"];
                t4.chooseC = Request.Form["t4C"];
                t4.chooseD = Request.Form["t4D"];
                t4.answer = Request.Form["t4answer"];
                t4.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~4";
                testlist.test4ID = t4.testId;
                Test t5 = new Test();
                t5.content = Request.Form["t5content"];
                t5.chooseA = Request.Form["t5A"];
                t5.chooseB = Request.Form["t5B"];
                t5.chooseC = Request.Form["t5C"];
                t5.chooseD = Request.Form["t5D"];
                t5.answer = Request.Form["t5answer"];
                t5.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~5";
                testlist.test5ID = t5.testId;
                Test t6 = new Test();
                t6.content = Request.Form["t6content"];
                t6.chooseA = Request.Form["t6A"];
                t6.chooseB = Request.Form["t6B"];
                t6.chooseC = Request.Form["t6C"];
                t6.chooseD = Request.Form["t6D"];
                t6.answer = Request.Form["t6answer"];
                t6.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~6";
                testlist.test6ID = t6.testId;
                Test t7 = new Test();
                t7.content = Request.Form["t7content"];
                t7.chooseA = Request.Form["t7A"];
                t7.chooseB = Request.Form["t7B"];
                t7.chooseC = Request.Form["t7C"];
                t7.chooseD = Request.Form["t7D"];
                t7.answer = Request.Form["t7answer"];
                t7.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~7";
                testlist.test7ID = t7.testId;
                Test t8 = new Test();
                t8.content = Request.Form["t8content"];
                t8.chooseA = Request.Form["t8A"];
                t8.chooseB = Request.Form["t8B"];
                t8.chooseC = Request.Form["t8C"];
                t8.chooseD = Request.Form["t8D"];
                t8.answer = Request.Form["t8answer"];
                t8.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~8";
                testlist.test8ID = t8.testId;
                Test t9 = new Test();
                t9.content = Request.Form["t9content"];
                t9.chooseA = Request.Form["t9A"];
                t9.chooseB = Request.Form["t9B"];
                t9.chooseC = Request.Form["t9C"];
                t9.chooseD = Request.Form["t9D"];
                t9.answer = Request.Form["t9answer"];
                t9.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~9";
                testlist.test9ID = t9.testId;
                Test t10 = new Test();
                t10.content = Request.Form["t10content"];
                t10.answer = Request.Form["t10answer"];
                t10.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~10";
                testlist.test10ID = t10.testId;
                db.tests.Add(t1);
                db.tests.Add(t2);
                db.tests.Add(t3);
                db.tests.Add(t4);
                db.tests.Add(t5);
                db.tests.Add(t6);
                db.tests.Add(t7);
                db.tests.Add(t8);
                db.tests.Add(t9);
                db.tests.Add(t10);
                db.testLists.Add(testlist);
                db.SaveChanges();
                ViewData["test1"] = t1;
                ViewData["test2"] = t2;
                ViewData["test3"] = t3;
                ViewData["test4"] = t4;
                ViewData["test5"] = t5;
                ViewData["test6"] = t6;
                ViewData["test7"] = t7;
                ViewData["test8"] = t8;
                ViewData["test9"] = t9;
                ViewData["test10"] = t10;
                ViewData["type"] = "result";
                return View(course);
                //  testlist.test10ID=
            }

            //  db.SaveChanges();

            return View(course);
        }
        public ActionResult EditTest(int id = 0)
        {
            
            Course course = db.Courses.Find(id);
            if (hasTrainee(course.TeacherID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            int listCount = db.testLists.Where(m => m.courseID == id).Count();
            string tid1 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test1ID;
            string tid2 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test2ID;
            string tid3 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test3ID;
            string tid4 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test4ID;
            string tid5 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test5ID;
            string tid6 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test6ID;
            string tid7 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test7ID;
            string tid8 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test8ID;
            string tid9 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test9ID;
            string tid10 = db.testLists.FirstOrDefault(t => (t.courseID == id && t.editTime == listCount)).test10ID;
            Test t1 = db.tests.FirstOrDefault(t => t.testId == tid1);
            Test t2 = db.tests.FirstOrDefault(t => t.testId == tid2);
            Test t3 = db.tests.FirstOrDefault(t => t.testId == tid3);
            Test t4 = db.tests.FirstOrDefault(t => t.testId == tid4);
            Test t5 = db.tests.FirstOrDefault(t => t.testId == tid5);
            Test t6 = db.tests.FirstOrDefault(t => t.testId == tid6);
            Test t7 = db.tests.FirstOrDefault(t => t.testId == tid7);
            Test t8 = db.tests.FirstOrDefault(t => t.testId == tid8);
            Test t9 = db.tests.FirstOrDefault(t => t.testId == tid9);
            Test t10 = db.tests.FirstOrDefault(t => t.testId == tid10);
            ViewData["test1"] = t1;
            ViewData["test2"] = t2;
            ViewData["test3"] = t3;
            ViewData["test4"] = t4;
            ViewData["test5"] = t5;
            ViewData["test6"] = t6;
            ViewData["test7"] = t7;
            ViewData["test8"] = t8;
            ViewData["test9"] = t9;
            ViewData["test10"] = t10;
            ViewData["type"] = "edit";

            return View(course);
        }

        [HttpPost]
        public ActionResult EditTest(Course course)
        {
            if (hasTrainee(course.TeacherID))
            {
                ViewData["hasTrainee"] = 1;
            }
            else
            {
                ViewData["hasTrainee"] = 0;
            }
            if (ModelState.IsValid)
            {
                testList testlist = new testList();
                testlist.courseID = course.ID;
                int editcount = db.testLists.Where(t => t.courseID == course.ID).ToList().Count;
                testlist.editTime = editcount > 0 ? (editcount + 1) : 1;
                Test t1 = new Test();
                t1.content = Request.Form["t1content"];
                t1.chooseA = Request.Form["t1A"];
                t1.chooseB = Request.Form["t1B"];
                t1.chooseC = Request.Form["t1C"];
                t1.chooseD = Request.Form["t1D"];
                t1.answer = Request.Form["t1answer"];
                t1.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~1";
                testlist.test1ID = t1.testId;
                Test t2 = new Test();
                t2.content = Request.Form["t2content"];
                t2.chooseA = Request.Form["t2A"];
                t2.chooseB = Request.Form["t2B"];
                t2.chooseC = Request.Form["t2C"];
                t2.chooseD = Request.Form["t2D"];
                t2.answer = Request.Form["t2answer"];
                t2.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~2";
                testlist.test2ID = t2.testId;
                Test t3 = new Test();
                t3.content = Request.Form["t3content"];
                t3.chooseA = Request.Form["t3A"];
                t3.chooseB = Request.Form["t3B"];
                t3.chooseC = Request.Form["t3C"];
                t3.chooseD = Request.Form["t3D"];
                t3.answer = Request.Form["t3answer"];
                t3.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~3";
                testlist.test3ID = t3.testId;
                Test t4 = new Test();
                t4.content = Request.Form["t4content"];
                t4.chooseA = Request.Form["t4A"];
                t4.chooseB = Request.Form["t4B"];
                t4.chooseC = Request.Form["t4C"];
                t4.chooseD = Request.Form["t4D"];
                t4.answer = Request.Form["t4answer"];
                t4.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~4";
                testlist.test4ID = t4.testId;
                Test t5 = new Test();
                t5.content = Request.Form["t5content"];
                t5.chooseA = Request.Form["t5A"];
                t5.chooseB = Request.Form["t5B"];
                t5.chooseC = Request.Form["t5C"];
                t5.chooseD = Request.Form["t5D"];
                t5.answer = Request.Form["t5answer"];
                t5.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~5";
                testlist.test5ID = t5.testId;
                Test t6 = new Test();
                t6.content = Request.Form["t6content"];
                t6.chooseA = Request.Form["t6A"];
                t6.chooseB = Request.Form["t6B"];
                t6.chooseC = Request.Form["t6C"];
                t6.chooseD = Request.Form["t6D"];
                t6.answer = Request.Form["t6answer"];
                t6.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~6";
                testlist.test6ID = t6.testId;
                Test t7 = new Test();
                t7.content = Request.Form["t7content"];
                t7.chooseA = Request.Form["t7A"];
                t7.chooseB = Request.Form["t7B"];
                t7.chooseC = Request.Form["t7C"];
                t7.chooseD = Request.Form["t7D"];
                t7.answer = Request.Form["t7answer"];
                t7.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~7";
                testlist.test7ID = t7.testId;
                Test t8 = new Test();
                t8.content = Request.Form["t8content"];
                t8.chooseA = Request.Form["t8A"];
                t8.chooseB = Request.Form["t8B"];
                t8.chooseC = Request.Form["t8C"];
                t8.chooseD = Request.Form["t8D"];
                t8.answer = Request.Form["t8answer"];
                t8.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~8";
                testlist.test8ID = t8.testId;
                Test t9 = new Test();
                t9.content = Request.Form["t9content"];
                t9.chooseA = Request.Form["t9A"];
                t9.chooseB = Request.Form["t9B"];
                t9.chooseC = Request.Form["t9C"];
                t9.chooseD = Request.Form["t9D"];
                t9.answer = Request.Form["t9answer"];
                t9.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~9";
                testlist.test9ID = t9.testId;
                Test t10 = new Test();
                t10.content = Request.Form["t10content"];
                t10.answer = Request.Form["t10answer"];
                t10.testId = testlist.editTime.ToString() + "~" + course.ID.ToString() + "~10";
                testlist.test10ID = t10.testId;
                db.tests.Add(t1);
                db.tests.Add(t2);
                db.tests.Add(t3);
                db.tests.Add(t4);
                db.tests.Add(t5);
                db.tests.Add(t6);
                db.tests.Add(t7);
                db.tests.Add(t8);
                db.tests.Add(t9);
                db.tests.Add(t10);
                db.testLists.Add(testlist);
                db.SaveChanges();
                ViewData["test1"] = t1;
                ViewData["test2"] = t2;
                ViewData["test3"] = t3;
                ViewData["test4"] = t4;
                ViewData["test5"] = t5;
                ViewData["test6"] = t6;
                ViewData["test7"] = t7;
                ViewData["test8"] = t8;
                ViewData["test9"] = t9;
                ViewData["test10"] = t10;
                ViewData["type"] = "result";
                return View(course);
            }
            return View(course);
        }
        public ActionResult Delete(int id = 0)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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