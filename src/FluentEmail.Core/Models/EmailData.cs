using System.Collections.Generic;

namespace FluentEmail.Core.Models
{
    /// <summary>
    /// 邮件数据
    /// </summary>
    public class EmailData
    {
        /// <summary>
        /// 收件人地址
        /// </summary>
        public List<Address> ToAddresses { get; set; }
        /// <summary>
        /// 抄送地址
        /// </summary>
        public List<Address> CcAddresses { get; set; }
        /// <summary>
        /// 密件抄送地址
        /// </summary>
        public List<Address> BccAddresses { get; set; }
        /// <summary>
        /// 回复地址
        /// </summary>
        public List<Address> ReplyToAddresses { get; set; }
        /// <summary>
        /// 附件
        /// </summary>
        public List<Attachment> Attachments { get; set; }
        /// <summary>
        /// 发件人地址
        /// </summary>
        public Address FromAddress { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 纯文本替代体
        /// </summary>
        public string PlaintextAlternativeBody { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public Priority Priority { get; set; }
        /// <summary>
        /// 标签
        /// </summary>
        public List<string> Tags { get; set; }
        /// <summary>
        /// 是否是Html
        /// </summary>
        public bool IsHtml { get; set; }
        /// <summary>
        /// 标头
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        public EmailData()
        {
            ToAddresses = new List<Address>();
            CcAddresses = new List<Address>();
            BccAddresses = new List<Address>();
            ReplyToAddresses = new List<Address>();
            Attachments = new List<Attachment>();
            Tags = new List<string>();
            Headers = new Dictionary<string, string>();
        }
    }
}
