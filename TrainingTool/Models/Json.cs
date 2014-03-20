using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace json.Models
{
    public class JsonBinder<T> : System.Web.Mvc.IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
        {
            System.Console.WriteLine("controllerContext:"+controllerContext.ToString());
            System.Console.WriteLine("bindingContext:" + bindingContext.ToString());
            //从请求中获取提交的参数数据 
            var json = controllerContext.HttpContext.Request.Form[bindingContext.ModelName] as string;
            //提交参数是对象 
            if (json!=null&&json.StartsWith("{") && json.EndsWith("}"))
            {
                JObject jsonBody = JObject.Parse(json);
                JsonSerializer js = new JsonSerializer();
                object obj = js.Deserialize(jsonBody.CreateReader(), typeof(T));
                return obj;
            }
            //提交参数是数组 
            if (json != null && json.StartsWith("[") && json.EndsWith("]"))
            {
                IList<T> list = new List<T>();
                JArray jsonRsp = JArray.Parse(json);
                if (jsonRsp != null)
                {
                    for (int i = 0; i < jsonRsp.Count; i++)
                    {
                        JsonSerializer js = new JsonSerializer();
                        try
                        {
                            object obj = js.Deserialize(jsonRsp[i].CreateReader(), typeof(T));
                            list.Add((T)obj);
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
                return list;
            }
            return null;
        }
        /*
                object BindModel(ControllerContext controllerContext, System.Web.ModelBinding.ModelBindingContext bindingContext)
                {
                    throw new NotImplementedException();
                }
                object BindModel(ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
                {
                    throw new NotImplementedException();
                }*/
    }
   

}