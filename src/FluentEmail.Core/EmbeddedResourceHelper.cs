using System.IO;
using System.Reflection;

namespace FluentEmail.Core
{
    /// <summary>
    /// 嵌入式资源助手
    /// </summary>
    internal static class EmbeddedResourceHelper
    {
        /// <summary>
        /// 以字符串形式获取资源
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string GetResourceAsString(Assembly assembly, string path)
        {
            string result;

            using (var stream = assembly.GetManifestResourceStream(path))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }
    }
}
