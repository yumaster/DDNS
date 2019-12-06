using System.IO;

namespace FluentEmail.Core.Models
{
    /// <summary>
    /// 附件
    /// </summary>
    public class Attachment
    {
        /// <summary>
        /// 获取或设置附件是否打算用于嵌入式图像（更改诸如MailGun之类的提供程序的参数名）
        /// </summary>
        public bool IsInline { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string Filename { get; set; }
        /// <summary>
        /// IO流
        /// </summary>
        public Stream Data { get; set; }
        /// <summary>
        /// /内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 内容ID
        /// </summary>
        public string ContentId { get; set; }
    }
}
