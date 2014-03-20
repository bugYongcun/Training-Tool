using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingTool.Models;
using System.Data.Entity;

namespace TrainingTool.Repository
{
    public class UserRepository: RepositoryBase<User>
    {
        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="Id">用户Id</param>
        /// <returns></returns>
        public override User Find(string EmployeeId)
        {
            return TrainingTooldbContext.User.SingleOrDefault(u => u.EmployeeId == EmployeeId);
        }
        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="UserName">用户名</param>
        /// <returns></returns>
        public User FindName(string UserName) 
        {
            return TrainingTooldbContext.User.SingleOrDefault(u => u.UserName == UserName);
        }
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        public bool Exists(string EmployeeId)
        {
            if (TrainingTooldbContext.User.Any(u => u.EmployeeId.ToUpper() == EmployeeId.ToUpper())) return true;
            else return false;
        }

        /// <summary>
        /// 用户验证【0-成功；1-用户名不存在；2-密码错误】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWrod"></param>
        /// <returns></returns>
        public int Authentication(string EmployeeId, string PassWord)
        {
            var _user = TrainingTooldbContext.User.SingleOrDefault(u => u.EmployeeId == EmployeeId);
            if (_user == null) return 1;
            else if (_user.Password != PassWord) return 2;
            else return 0;
        }
    }
}