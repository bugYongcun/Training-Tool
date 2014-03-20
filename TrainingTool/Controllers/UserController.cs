using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using PagedList;
using PagedList.Mvc;
using TrainingTool.Models;
using System.Drawing;
using TrainingTool.Repository;

namespace TrainingTool.Controllers
{
    public class UserController : Controller
    {
        private TrainingToolContext db = new TrainingToolContext();
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalCount = 0;

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin login)
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
            UserRepository userRsy = new UserRepository();
            if (userRsy.Authentication(login.EmployeeId, login.Password) == 0)
            {
                HttpCookie _cookie = new HttpCookie("User");
                _cookie.Values.Add("EmployeeId", login.EmployeeId);
                _cookie.Values.Add("Password", login.Password);
                Response.Cookies.Add(_cookie);
                return RedirectToAction("Index", "Course", new { lastTime = DateTime.Now, userId = login.EmployeeId });
            }
            else
            {
                ModelState.AddModelError("Message", "登陆失败！");
                return View();
            }

        }

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            if (Request.Cookies["User"] != null)
            {
                HttpCookie _cookie = Request.Cookies["User"];
                _cookie.Expires = DateTime.Now.AddHours(-5);
                Response.Cookies.Add(_cookie);
            }
            Notice _n = new Notice { Title = "成功退出", Details = "您已经成功退出！", DwellTime = 5, NavigationName = "网站首页", NavigationUrl = Url.Action("Index", "Home") };
            return RedirectToAction("Notice", "Prompt", _n);
        }

        [UserAuthorizeAttribute]
        public ActionResult Default()
        {
            UserRepository userRsy = new UserRepository();
            var _user = userRsy.Find(UserEmployeeId);
            return View(_user);
        }

        [AdminAuthorizeAttribute]
        public ActionResult Index(int page = 1)
        {
            currentPage = (page - 1 > 0) ? page : 1;
            var users = GetUser(currentPage, pageSize, ref totalCount);
            var usersAsIPageList = new StaticPagedList<User>(users, currentPage, pageSize, totalCount);

            AdminRepository adminRsy = new AdminRepository();
            var _admin = adminRsy.Find(AdminEmployeeId);
            ViewBag.admin = _admin;
            return View(usersAsIPageList);
        }

        [HttpPost]
        public ActionResult Index(string EmployeeId, string UserName)
        {
            var users = from m in db.User
                        select m;

            if (!String.IsNullOrEmpty(EmployeeId))
            {
                users = users.Where(s => s.EmployeeId.Contains(EmployeeId));
            }

            if (string.IsNullOrEmpty(UserName))
            {
                users.Skip((currentPage - 1) * pageSize).Take(pageSize);
                totalCount = users.Count();
                var usersAsIPageList = new StaticPagedList<User>(users.ToList(), currentPage, pageSize, totalCount);
                return View(usersAsIPageList);
            }
            else
            {
                users = users.Where(x => x.UserName.Contains(UserName));
                users.Skip((currentPage - 1) * pageSize).Take(pageSize);
                totalCount = users.Count();
                var usersAsIPageList = new StaticPagedList<User>(users.ToList(), currentPage, pageSize, totalCount);
                return View(usersAsIPageList);
            }

        }

        public List<User> GetUser(int pageIndex, int pageSize, ref int totalCount)
        {
            var user = (from p in db.User
                        orderby p.Id descending
                        select p).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            totalCount = db.User.Count();
            return user.ToList();

        }

        //
        // GET: /User/Details/5

        [AdminAuthorizeAttribute]
        public ActionResult Details(int id = 0)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // GET: /User/Create

        [AdminAuthorizeAttribute]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            UserRepository userRsy = new UserRepository();
            if (userRsy.Exists(user.EmployeeId))
            {
                ModelState.AddModelError("EmployeeId", "The Employee ID have existed");
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {       
                db.User.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(user);
        }

        //
        // GET: /User/Edit/5

        [AdminAuthorizeAttribute]
        public ActionResult Edit(int id = 0)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        [AdminAuthorizeAttribute]
        public ActionResult Delete(int id = 0)
        {
            User user = db.User.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.User.Find(id);
            db.User.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// 绘制验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult VerificationCode()
        {
            int _verificationLength = 6;
            int _width = 100, _height = 20;
            SizeF _verificationTextSize;
            Bitmap _bitmap = new Bitmap(Server.MapPath("~/Images/Texture.jpg"), true);
            TextureBrush _brush = new TextureBrush(_bitmap);
            //获取验证码
            string _verificationText = Common.Text.VerificationText(_verificationLength);
            //存储验证码
            Session["VerificationCode"] = _verificationText.ToUpper();
            Font _font = new Font("Arial", 14, FontStyle.Bold);
            Bitmap _image = new Bitmap(_width, _height);
            Graphics _g = Graphics.FromImage(_image);
            //清空背景色
            _g.Clear(Color.White);
            //绘制验证码
            _verificationTextSize = _g.MeasureString(_verificationText, _font);
            _g.DrawString(_verificationText, _font, _brush, (_width - _verificationTextSize.Width) / 2, (_height - _verificationTextSize.Height) / 2);
            _image.Save(Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            return null;
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        public string UserEmployeeId
        {
            get
            {
                HttpCookie _cookie = Request.Cookies["User"];
                if (_cookie == null) return "";
                else return _cookie["EmployeeId"];
            }
        }

        /// <summary>
        /// 获取用户名
        /// </summary>
        public string AdminEmployeeId
        {
            get
            {
                HttpCookie _cookie = Request.Cookies["Admin"];
                if (_cookie == null) return "";
                else return _cookie["EmployeeId"];
            }
        }
    }
}