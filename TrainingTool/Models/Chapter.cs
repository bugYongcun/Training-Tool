using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingTool.Models
{
    public class Chapter
    {
        public int ID { get; set; }
        public int courseID { get; set; }
        public int chapterID{ get; set; }
        public string Name { get; set; }
        public string contentFile { get; set; }
        public string videoFile { get; set; }
    }
}