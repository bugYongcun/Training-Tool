using System.Web;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
namespace TrainingTool.Models
{
    public class userAnswer
    {
        public int ID { get; set; }
        public string userId { get; set; }
        public int courseId { get; set; }
        public string t1Answer { get; set; }
        public string t2Answer { get; set; }
        public string t3Answer { get; set; }
        public string t4Answer { get; set; }
        public string t5Answer { get; set; }
        public string t6Answer { get; set; }
        public string t7Answer { get; set; }
        public string t8Answer { get; set; }
        public string t9Answer { get; set; }
        public string t10Answer { get; set; }
    }
}