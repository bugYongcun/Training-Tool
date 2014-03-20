using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainingTool.Models;

namespace TrainingTool.Controllers
{
    /// <summary>
    /// 提醒
    /// </summary>
    public class PromptController : Controller
    {
        //
        // GET: /Note/

        public ActionResult Notice(Notice notice)
        {
            return View(notice);
        }
        public ActionResult Error(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }
        public ActionResult UserNotice(Notice notice)
        {
            return View(notice);
        }
        public ActionResult UserError(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }
        public ActionResult ManageNotice(Notice notice)
        {
            return View(notice);
        }
        public ActionResult ManageError(Error error)
        {
            error.Details = Server.UrlDecode(error.Details);
            error.Cause = Server.UrlDecode(error.Cause);
            error.Solution = Server.UrlDecode(error.Solution);
            return View(error);
        }
    }
}
