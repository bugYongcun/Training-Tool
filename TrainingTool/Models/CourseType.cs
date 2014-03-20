using System.Web;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingTool.Models
{
    public class CourseType
    {
        public int ID { get; set; }
        public string typeID { get; set; }
        public string Name { get; set; }

    }
}