using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Interfaces
{
    /// <summary>
    /// 发件人接口
    /// </summary>
    public interface ISender
    {
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        SendResponse Send(IFluentEmail email, CancellationToken? token = null);
        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null);
    }
}
