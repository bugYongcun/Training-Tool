using System.Web;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;

namespace TrainingTool.Models
{
    public class Depart
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public double Mark { get; set; }
    }
}