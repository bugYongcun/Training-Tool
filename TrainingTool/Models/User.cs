using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Data.Entity;

namespace TrainingTool.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 工号 
        /// </summary>
        [Required(ErrorMessage = "×")]
        [Display (Name = "Employee ID")]
        public string EmployeeId { get; set; }

        /// <summary>
        /// 用户组Id
        /// </summary>
        [Display(Name = "Department ID")]
        [Required(ErrorMessage = "×")]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "Your True Name", Description = "4-20 characters.")]
        [Required(ErrorMessage = "×")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "×")]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "Password", Description = "6-20 characters.")]
        [Required(ErrorMessage = "×")]
        [StringLength(512, MinimumLength = 6, ErrorMessage = "×")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 性别【0-男；1-女；2-保密】
        /// </summary>
        [Display(Name = "Gender")]
        [Required(ErrorMessage = "×")]
        [Range(0, 2, ErrorMessage = "×")]
        public byte Gender { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        [Display(Name = "Email", Description = "Please enter your common Email。")]
        [Required(ErrorMessage = "×")]
        [EmailAddress(ErrorMessage = "×")]
        public string Email { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [Display(Name = "Phone Number", Description = "Commonly used telephone (mobile or fixed), fixed format: area code - number.")]
        [RegularExpression("^0{0,1}(13[0-9]|15[0-9])[0-9]{8}$", ErrorMessage = "×")]
        public string Tel { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime? RegTime { get; set; }
        /// <summary>
        /// 上次登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

    }

    /// <summary>
    /// 用户登陆模型
    /// </summary>
    public class UserLogin
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "×")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "×")]
        public string EmployeeId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "Password", Description = "6-20 characters")]
        [Required(ErrorMessage = "×")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "×")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "Verification Code")]
        [Required(ErrorMessage = "×")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "×")]
        public string VerificationCode { get; set; }

    }
}