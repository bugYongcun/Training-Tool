using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainingTool.Repository
{
    public class RepositoryBase<TModel>
    {
        protected TrainingToolContext TrainingTooldbContext;
        public RepositoryBase()
        {
            TrainingTooldbContext = new TrainingToolContext();
        }
        /// <summary>
        /// 添加【继承类重写后才能正常使用】
        /// </summary>
        public virtual bool Add(TModel Tmodel) { return false; }
        /// <summary>
        /// 更新【继承类重写后才能正常使用】
        /// </summary>
        public virtual bool Update(TModel Tmodel) { return false; }
        /// <summary>
        /// 删除【继承类重写后才能正常使用】
        /// </summary>
        public virtual bool Delete(int Id) { return false; }
        /// <summary>
        /// 查找指定值【继承类重写后才能正常使用】
        /// </summary>
        public virtual TModel Find(string Id) { return default(TModel); }
        ~RepositoryBase()
        {
            if (TrainingTooldbContext != null)
            {
                TrainingTooldbContext.Dispose();
            }
        }
    }
}