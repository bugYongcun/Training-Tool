using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingTool.Models
{
    public class taskRecord
    {
        public int ID { get; set; }
        public int courseID { get; set; }
        public string userID { get; set; }
        public string assignBy { get; set; }
        public DateTime taskTime { get; set; }
        public int readChapter { get; set; }
        public double learnMinutes { get; set; }
    }
}