using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TrainingTool.Models
{
    public class CourseList
    {
        public int ID { get; set; }
        public Course course { get; set; }
        public User user { get; set; }
        public CourseType type { get; set; }
    }
    
}