using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Data.Entity;

namespace TrainingTool.Models
{
    public class TrainerToTrainee
    {
        public int Id { get; set; }
        public string trainerId { get; set; }
        public string traineeId { get; set; }
    }
}