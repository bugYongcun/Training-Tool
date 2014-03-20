using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainingTool.Models;
using System.Data.Entity;

namespace TrainingTool.Repository
{
    public class CourseApplyRepository: RepositoryBase<CourseApply>
    {
        public override CourseApply Find(string ApplyName)
        {
            return TrainingTooldbContext.CourseApplies.Find(ApplyName);
        }
    }
}