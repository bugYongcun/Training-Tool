using System;
using System.Collections.Generic;
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
    public class AdminController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();
        private int currentPage = 1;
        private int pageSize = 1;//10
        private int totalCount = 0;
        //
        // GET: /Admin/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AdminLogin login)
        {
            //验证验证码
            if (Session["VerificationCode"] == null || Session["VerificationCode"].ToString() == "")
            {
                Error _e = new Error { Title = "验证码不存在", Details = "在用户注册时，服务器端的验证码为空，或向服务器提交的验证码为空", Cause = "<li>你注册时在注册页面停留的时间过久页已经超时</li><li>您绕开客户端验证向服务器提交数据</li>", Solution = "返回<a href='" + Url.Action("Register", "User") + "'>注册</a>页面，刷新后重新注册" };
                return RedirectToAction("Error", "Prompt", _e);
            }
            else if (Session["VerificationCode"].ToString() != login.VerificationCode.ToUpper())
            {
                ModelState.AddModelError("VerificationCode", "×");
                return View();
            }
            //验证账号密码
            AdminRepository adminRsy = new AdminRepository();
            if (adminRsy.Authentication(login.EmployeeId, login.Password) == 0)
            {
                HttpCookie _cookie = new HttpCookie("Admin");
                _cookie.Values.Add("EmployeeId", login.EmployeeId);
                _cookie.Values.Add("Password", login.Password);
                Response.Cookies.Add(_cookie);
                return RedirectToAction("Index", "User");
            }
            else
            {
                ModelState.AddModelError("Message", "登陆失败！");
                return View();
            }
        }

        //
        // GET: /Admin/CourseApplyIndex

        public ActionResult CourseApplyIndex(int page = 1)
        {
            currentPage = (page - 1 > 0) ? page : 1;
            var courseApplies = GetCourseApply(currentPage, pageSize, ref totalCount);
            var courseAppliesAsIPageList = new StaticPagedList<CourseApply>(courseApplies, currentPage, pageSize, totalCount);
            return View(courseAppliesAsIPageList);
        }

        public List<CourseApply> GetCourseApply(int pageIndex, int pageSize, ref int totalCount)
        {
            var courseApply = (from p in db.CourseApplies
                        orderby p.ID descending
                        select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = db.CourseApplies.Count();
            return courseApply.ToList();

        }
        //
        // GET: /Admin/CourseApplyDetails/id

        public ActionResult CourseApplyDetails(int id = 0)
        {
            CourseApply courseApply = db.CourseApplies.Find(id);
            if (courseApply == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = id;
            return View(courseApply);
        }

        public ActionResult AgressApply(int id = 0)
        {
            CourseApply courseApply = db.CourseApplies.Find(id);
            if (courseApply == null)
            {
                return HttpNotFound();
            }

            Course course = new Course();
            course.Name = courseApply.CourseName;
            course.setDate = DateTime.Now;
            course.TeacherID = courseApply.TeacherID;
            course.ForDepart = courseApply.ForDepart;
            course.Type = courseApply.Type;
            course.Mark = 0.00;

            db.Courses.Add(course);
            db.SaveChanges();

            db.CourseApplies.Remove(courseApply);
            db.SaveChanges();

            return RedirectToAction("CourseApplyIndex", "Admin");
        }
        public ActionResult arrangeTrainee()
        {
            //View Model Object
            addTrainees vmd = new addTrainees();
            List<CheckBoxList> checkBoxList = new List<CheckBoxList>();
           List<User> allStuffs= db.User.ToList();
           foreach (User u in allStuffs)
           {
               CheckBoxList list = new CheckBoxList();
               list.stuffId = u.EmployeeId;
               list.name = u.UserName;
               list.Tel = u.Tel;
               list.Email = u.Email;
               list.department = u.DepartmentId;
               switch (u.Gender)
               {
                   case 0: list.gender = "male";
                       break;
                   case 1: list.gender = "female";
                       break;
                   default: list.gender = "unknown";
                       break;
               }
              
               checkBoxList.Add(list);
           }

            vmd.checkBoxList = checkBoxList;
            return View(vmd);
        }
        [HttpPost]
        public ActionResult arrangeTrainee(addTrainees vm)
        {
            foreach (var item in vm.checkBoxList)
            {
                if (item.Selected)
                {
                    TrainerToTrainee newArrange=new TrainerToTrainee();
                    newArrange.traineeId=item.stuffId;
                    newArrange.trainerId=Request.Form["Trainer"];
                    db.trainerTotrainee.Add(newArrange);
                }
            }
            db.SaveChanges();
            return View(vm);
        }

        public ActionResult adminViewTrainee(string trainerID, int page = 1)
        {
           List<TrainerToTrainee> pairs=db.trainerTotrainee.Where(t => t.trainerId == trainerID).ToList();
            List<User> trainees=new List<User>();
           foreach (var p in pairs)
           {
              User trainee= db.User.FirstOrDefault(u => u.EmployeeId == p.traineeId);
              trainees.Add(trainee);
           }
           ViewData["trainer"] = trainerID;

        /*   if (!String.IsNullOrEmpty(Keyword))
           {
               books = books.Where(b =>
               b.Title.ToUpper().Contains(Keyword.ToUpper())
               || b.Author.ToUpper().Contains(
               Keyword.ToUpper()));
           }
           ViewBag.CurrentKeyword =
           String.IsNullOrEmpty(Keyword) ? "" : Keyword;*/

           return View(trainees);
      /*     List<Depart> departList = db.Departs.ToList();
           List<string> departNames = new List<string>();

           foreach (Depart d in departList)
           {
               departNames.Add(d.Name);
           }

           ViewData["list"] = departNames;
           currentPage = (page - 1 > 0) ? page : 1;
           var users = GetTrainee(currentPage, pageSize, ref totalCount,pairs);
           var usersAsIPageList = new StaticPagedList<User>(users, currentPage, pageSize, totalCount);

           return View(usersAsIPageList);*/
        }
        public List<User> GetTrainee(int pageIndex, int pageSize, ref int totalCount,List<TrainerToTrainee> pairs)
        {
            IQueryable<User> temp = null;
             foreach (var a in pairs)
             {
            var user = (from p in db.User
                        where p.EmployeeId== a.traineeId
                        orderby p.Id descending
                        select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            temp = temp.Union(user);
             }
             totalCount = pairs.Count;
             return temp.ToList();

        }
      /*  [HttpPost]
        public ActionResult arrangeTrainee(addTrainees vm)
        {
            foreach (var item in vm.checkBoxList)
            {
                if (item.Selected)
                {
                    TrainerToTrainee newArrange=new TrainerToTrainee();
                    newArrange.traineeId=item.stuffId;
                    newArrange.trainerId=Request.Form["Trainer"];
                    db.trainerTotrainee.Add(newArrange);
                }
            }
            db.SaveChanges();
            return View(vm);
        }*/
       // [HttpPost]
        public ActionResult DeleteTrainee(string traineeID, string trainerID)
        {
          /*  TrainerToTrainee pair = newPair;
            string traineeID = pair.traineeId;
            string trainerID = pair.trainerId;*/
            TrainerToTrainee record=db.trainerTotrainee.FirstOrDefault(t=>t.traineeId==traineeID&&t.trainerId==trainerID);
            db.trainerTotrainee.Remove(record);
            db.SaveChanges();
            ViewData["trainer"] = trainerID;
            List<Depart> departList = db.Departs.ToList();
            List<string> departNames = new List<string>();

            foreach (Depart d in departList)
            {
                departNames.Add(d.Name);
            }

            ViewData["list"] = departNames;
            return RedirectToAction("adminViewTrainee", new { trainerID=trainerID });
        }
        public ActionResult AddTrainee(string trainerID)
        {
            ViewData["trainer"] = trainerID;
            addTrainees vmd = new addTrainees();
            List<CheckBoxList> checkBoxList = new List<CheckBoxList>();
            List<TrainerToTrainee> traineelist = db.trainerTotrainee.Where(t => t.trainerId == trainerID).ToList();
            List<User> trainees = new List<Models.User>();
            foreach (var u in traineelist)
            {
               User trainee= db.User.FirstOrDefault(m => m.EmployeeId == u.traineeId);
               trainees.Add(trainee);
            }
            //List<User> notrainees = new List<Models.User>();
            List<User> allStuffs = db.User.ToList();
            foreach (User u in trainees)
            {
                allStuffs.Remove(u);
            }
            foreach (User u in allStuffs)
            {
                CheckBoxList list = new CheckBoxList();
                list.stuffId = u.EmployeeId;
                list.name = u.UserName;
                list.Tel = u.Tel;
                list.Email = u.Email;
                list.department = u.DepartmentId;
                switch (u.Gender)
                {
                    case 0: list.gender = "male";
                        break;
                    case 1: list.gender = "female";
                        break;
                    default: list.gender = "unknown";
                        break;
                }

                checkBoxList.Add(list);
            }

            vmd.checkBoxList = checkBoxList;
            return View(vmd);
        }
        [HttpPost]
        public ActionResult AddTrainee(addTrainees vm, string trainerID)
        {
            foreach (var item in vm.checkBoxList)
            {
                if (item.Selected)
                {
                    TrainerToTrainee newArrange = new TrainerToTrainee();
                    newArrange.traineeId = item.stuffId;
                    newArrange.trainerId = trainerID;
                    db.trainerTotrainee.Add(newArrange);
                }
            }
            db.SaveChanges();
            ViewData["trainer"] = trainerID;
            List<Depart> departList = db.Departs.ToList();
            List<string> departNames = new List<string>();

            foreach (Depart d in departList)
            {
                departNames.Add(d.Name);
            }

            ViewData["list"] = departNames;
            return RedirectToAction("adminViewTrainee", new { trainerID = trainerID });
        }

    }
     
    public class addTrainees
    {
        public List<CheckBoxList> checkBoxList { get; set; }
        public string trainerId { get; set; }
    }

    public class CheckBoxList
    {
        public string stuffId { get; set; }
       public string department { get; set; }
        public string name { get; set; }
        public string gender { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public bool Selected { get; set; }
    }
}
