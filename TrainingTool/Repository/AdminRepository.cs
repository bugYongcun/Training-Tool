using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingTool.Models;
using System.Data.Entity;

namespace TrainingTool.Repository
{
    public class AdminRepository: RepositoryBase<Administrator>
    {
        /// <summary>
        /// 查找用户
        /// </summary>
        /// <param name="Id">用户Id</param>
        /// <returns></returns>
        public override Administrator Find(string EmployeeId)
        {
            return TrainingTooldbContext.Administrators.SingleOrDefault(a => a.EmployeeId == EmployeeId);
        }

        /// <summary>
        /// 用户验证【0-成功；1-用户名不存在；2-密码错误】
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="PassWrod"></param>
        /// <returns></returns>
        public int Authentication(string EmployeeId, string PassWord)
        {
            var _admin = TrainingTooldbContext.Administrators.SingleOrDefault(a => a.EmployeeId == EmployeeId);
            if (_admin == null) return 1;
            else if (_admin.Password != PassWord) return 2;
            else return 0;
        }
    }
}