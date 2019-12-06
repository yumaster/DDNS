using System.Reflection;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;

namespace FluentEmail.Core.Defaults
{
    /// <summary>
    /// 更换渲染器
    /// </summary>
    public class ReplaceRenderer : ITemplateRenderer
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public string Parse<T>(string template, T model, bool isHtml = true)
        {
            foreach (PropertyInfo pi in model.GetType().GetRuntimeProperties())
            {
                template = template.Replace($"##{pi.Name}##", pi.GetValue(model, null).ToString());
            }

            return template;            
        }
        /// <summary>
        /// 异步解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public Task<string> ParseAsync<T>(string template, T model, bool isHtml = true)
        {
            return Task.FromResult(Parse(template, model, isHtml));
        }
    }
}
