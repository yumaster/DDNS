using System.Collections.Generic;
using System.Linq;

namespace FluentEmail.Core.Models
{
    /// <summary>
    /// 发送回复
    /// </summary>
    public class SendResponse
    {
        /// <summary>
        /// 讯息编号
        /// </summary>
        public string MessageId { get; set; }
        /// <summary>
        /// 错误讯息
        /// </summary>
        public List<string> ErrorMessages { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Successful => !ErrorMessages.Any();

        public SendResponse()
        {
            ErrorMessages = new List<string>();
        }
    }

    public class SendResponse<T> : SendResponse
    {
        public T Data { get; set; }
    }
}