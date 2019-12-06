using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core.Defaults
{
    /// <summary>
    /// 保存到磁盘发件人
    /// </summary>
    public class SaveToDiskSender : ISender
    {
        /// <summary>
        /// 目录
        /// </summary>
        private readonly string _directory;

        public SaveToDiskSender(string directory)
        {
            _directory = directory;
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public SendResponse Send(IFluentEmail email, CancellationToken? token = null)
        {
            //异步发送
            return SendAsync(email, token).GetAwaiter().GetResult();
        }
        /// <summary>
        /// 异步发送
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<SendResponse> SendAsync(IFluentEmail email, CancellationToken? token = null)
        {
            var response = new SendResponse();
            await SaveEmailToDisk(email);
            return response;
        }
        /// <summary>
        /// 保存电子邮件到磁盘
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private async Task<bool> SaveEmailToDisk(IFluentEmail email)
        {
            var random = new Random();
            var filename = $"{_directory.TrimEnd('\\')}\\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{random.Next(1000)}";

            using (var sw = new StreamWriter(File.OpenWrite(filename)))
            {
                sw.WriteLine($"From: {email.Data.FromAddress.Name} <{email.Data.FromAddress.EmailAddress}>");
                sw.WriteLine($"To: {string.Join(",", email.Data.ToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Cc: {string.Join(",", email.Data.CcAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Bcc: {string.Join(",", email.Data.BccAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"ReplyTo: {string.Join(",", email.Data.ReplyToAddresses.Select(x => $"{x.Name} <{x.EmailAddress}>"))}");
                sw.WriteLine($"Subject: {email.Data.Subject}");
                foreach (var dataHeader in email.Data.Headers)
                {
                    sw.WriteLine($"{dataHeader.Key}:{dataHeader.Value}");
                }
                sw.WriteLine();
                await sw.WriteAsync(email.Data.Body);
            }

            return true;
        }
    }
}
