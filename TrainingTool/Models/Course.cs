using System.Web;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TrainingTool.Models
{
    //和数据库中的表对应，一个对像对应表中一条记录
    public class Course
    {
        public int ID { get; set; }//可以自动分配id，递增或。。。

        public DateTime setDate { get; set; }  //课程建立时间
        public string TeacherID { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }  //课程类型需要另外建一个表吗？多级类型。。。此strin是ID还是类型内容
        public string Introduction { get; set; }
       
        public string ForDepart { get; set; }
        [Display(Name = "Upload contents:")]
        public int markCount { get; set; }
        public double Mark { get; set; }
    }

   
}