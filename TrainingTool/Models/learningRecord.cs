using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingTool.Models
{
    public class learningRecord
    {
        public int ID { get; set; }
        public int courseID { get; set; }
        public string userID { get; set; }
        public DateTime lastTime { get; set; }
    }
}