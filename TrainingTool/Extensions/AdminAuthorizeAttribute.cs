using TrainingTool.Repository;

namespace System.Web.Mvc
{
    /// <summary>
    ///  管理员权限验证
    /// </summary>
    public class AdminAuthorizeAttribute:AuthorizeAttribute
    {
        /// <summary>
        /// 核心【验证用户是否登陆】
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //检查Cookies["User"]是否存在
            if (httpContext.Request.Cookies["Admin"] == null) return false;
            //验证用户名密码是否正确
            HttpCookie _cookie = httpContext.Request.Cookies["Admin"];
            string _employeeId = _cookie["EmployeeId"];
            string _password = _cookie["Password"];
            if (_employeeId == "" || _password == "") return false;
            AdminRepository _userRsy = new AdminRepository();
            if (_userRsy.Authentication(_employeeId, _password) == 0) return true;
            else return false;
        }
    }
}