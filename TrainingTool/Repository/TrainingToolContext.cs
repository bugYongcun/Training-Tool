using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TrainingTool.Models;

namespace TrainingTool.Repository
{
    public class TrainingToolContext : DbContext
    {
        public TrainingToolContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<User> User { get; set; }
        public DbSet<Depart> Departs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<CourseList> courselists { get; set; }
        public DbSet<CourseApply> CourseApplies { get; set; }
        public DbSet<Administrator> Administrators { get; set; }
        public DbSet<learningApply> learningApplys { get; set; }
        public DbSet<learningRecord> learningRecords { get; set; }
        public DbSet<saveRecord> saveRecords { get; set; }
        public DbSet<taskRecord> taskRecords { get; set; }
        public DbSet<Test> tests { get; set; }
        public DbSet<testList> testLists { get; set; }
        public DbSet<Chapter> chapters { get; set; }
        public DbSet<TrainerToTrainee> trainerTotrainee { get; set; }
        public DbSet<userAnswer> userAnswers { get; set; }

    }
}