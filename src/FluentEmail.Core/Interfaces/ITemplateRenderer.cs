using System.Threading.Tasks;

namespace FluentEmail.Core.Interfaces
{
    /// <summary>
    /// 模板渲染器接口
    /// </summary>
    public interface ITemplateRenderer
    {
        /// <summary>
        /// 解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">模板</param>
        /// <param name="model">实体</param>
        /// <param name="isHtml">是否是Html</param>
        /// <returns></returns>
        string Parse<T>(string template, T model, bool isHtml = true);
        /// <summary>
        /// 异步解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="template">模板</param>
        /// <param name="model">实体</param>
        /// <param name="isHtml">是否是Html</param>
        /// <returns></returns>
        Task<string> ParseAsync<T>(string template, T model, bool isHtml = true);
    }
}
