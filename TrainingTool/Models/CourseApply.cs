using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace TrainingTool.Models
{
    public class CourseApply
    {
        [Key]
        public int ID { get; set; }//可以自动分配id，递增或。。。

        public DateTime ApplyDate { get; set; }  //课程申请时间

        public string TeacherID { get; set; }

        [Required(ErrorMessage = "×")]
        [Display(Name = "Course Name")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "×")]
        [Display(Name = "Type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "×")]
        [Display(Name = "For Departs")]
        public string ForDepart { get; set; }

        [Required(ErrorMessage = "×")]
        [StringLength(256, MinimumLength = 50, ErrorMessage = "Please don't less than 50 characters")]
        public string Introduction { get; set; }

        [StringLength(256, MinimumLength = 50, ErrorMessage = "Please don't less than 50 characters")]
        public string negativeOpinion { get; set; }
    }
}