using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Mvc.Html
{
    public static class DisplayDescriptionExtensions
    {
        /// <summary>
        /// 模型描述信息
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayDescription(this HtmlHelper htmlHelper, string name)
        {
            ModelMetadata _modelMetadata = ModelMetadata.FromStringExpression(name, htmlHelper.ViewData);
            return MvcHtmlString.Create(_modelMetadata.Description);
        }
        /// <summary>
        /// 模型描述信息
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayDescriptionFor<TModel, TResult>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TResult>> expression)
        {
            ModelMetadata _modelMetadata = ModelMetadata.FromLambdaExpression(expression,htmlHelper.ViewData);
            return MvcHtmlString.Create(_modelMetadata.Description);
        }
    }
}