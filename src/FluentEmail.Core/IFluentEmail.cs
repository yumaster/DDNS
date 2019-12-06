using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core
{
    /// <summary>
    /// 流利的电子邮件接口
    /// </summary>
    public interface IFluentEmail: IHideObjectMembers
	{
	    EmailData Data { get; set; }
	    ITemplateRenderer Renderer { get; set; }
	    ISender Sender { get; set; }

        /// <summary>
        /// 将收件人添加到电子邮件中，在“;”上拆分名称和地址
        /// </summary>
        /// <param name="emailAddress">收件人的电子邮件地址</param>
        /// <param name="name">收件人姓名</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail To(string emailAddress, string name = null);

        /// <summary>
        /// 设置发送自电子邮件地址
        /// </summary>
        /// <param name="emailAddress">发件人的电子邮件地址</param>
        /// <param name="name">发件人姓名</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail SetFrom(string emailAddress, string name = null);

        /// <summary>
        /// 将收件人添加到电子邮件
        /// </summary>
        /// <param name="emailAddress">收件人的电子邮件地址 (允许对';'进行多次拆分)</param>
        /// <returns></returns>
        IFluentEmail To(string emailAddress);

        /// <summary>
        /// 将列表中的所有收件人添加到电子邮件
        /// </summary>
        /// <param name="mailAddresses">收件人名单</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail To(IList<Address> mailAddresses);

        /// <summary>
        /// 将复本添加到电子邮件
        /// </summary>
        /// <param name="emailAddress">抄送电子邮件地址</param>
        /// <param name="name">抄送名称</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail CC(string emailAddress, string name = "");

        /// <summary>
        /// 将列表中的所有复本添加到电子邮件中
        /// </summary>
        /// <param name="mailAddresses">抄送的收件人列表</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail CC(IList<Address> mailAddresses);

        /// <summary>
        /// 在电子邮件中添加密件抄送
        /// </summary>
        /// <param name="emailAddress">密件抄送的电子邮件地址</param>
        /// <param name="name">密件抄送名称</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail BCC(string emailAddress, string name = "");

        /// <summary>
        /// 将列表中的所有密件抄送副本添加到电子邮件中
        /// </summary>
        /// <param name="mailAddresses">密件抄送收件人列表</param>
        /// <returns>Email类的实例</returns>
        IFluentEmail BCC(IList<Address> mailAddresses);

        /// <summary>
        /// 在电子邮件上设置回复地址
        /// </summary>
        /// <param name="address">回复地址</param>
        /// <returns></returns>
        IFluentEmail ReplyTo(string address);

        /// <summary>
        /// 在电子邮件上设置回复地址
        /// </summary>
        /// <param name="address">回复地址</param>
        /// <param name="name">回复的显示名称</param>
        /// <returns></returns>
        IFluentEmail ReplyTo(string address, string name);

        /// <summary>
        /// 设置电子邮件的主题
        /// </summary>
        /// <param name="subject">电子邮件主题</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail Subject(string subject);

        /// <summary>
        /// 将正文添加到电子邮件
        /// </summary>
        /// <param name="body">邮件正文</param>
        /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（可选）</param>
        IFluentEmail Body(string body, bool isHtml = false);

        /// <summary>
        /// 将电子邮件标记为高优先级
        /// </summary>
        IFluentEmail HighPriority();

        /// <summary>
        /// 将电子邮件标记为低优先级
        /// </summary>
        IFluentEmail LowPriority();

        /// <summary>
        /// 设置要使用的模板渲染引擎，默认为RazorEngine
        /// </summary>
        IFluentEmail UsingTemplateEngine(ITemplateRenderer renderer);

        /// <summary>
        /// 将模板添加到来自嵌入式资源的电子邮件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径嵌入式资源，例如[YourAssembly]。[YourResourceFolder]。[YourFilename.txt]</param>
        /// <param name="model">模板模型</param>
        /// <param name="assembly">资源所在的程序集。默认为调用程序集。</param>
        /// <returns></returns>
        IFluentEmail UsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly, bool isHtml = true);

        /// <summary>
        /// 将模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// <param name="model">模板模型</param>
	    /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（可选）</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail UsingTemplateFromFile<T>(string filename, T model, bool isHtml = true);

        /// <summary>
        /// 将特定于文化的模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// /// <param name="model">剃刀模型</param>
        /// <param name="culture">模板的区域性（默认为当前区域性）</param>
        /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（可选）</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture, bool isHtml = true);

        /// <summary>
        /// 将剃刀模板添加到电子邮件
        /// </summary>
        /// <param name="template">剃刀模板</param>
        /// <param name="model">模板模型</param>
	    /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（可选）</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail UsingTemplate<T>(string template, T model, bool isHtml = true);

        /// <summary>
        /// 将附件添加到电子邮件
        /// </summary>
        /// <param name="attachment">附件添加</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail Attach(Attachment attachment);

        /// <summary>
        /// 将多个附件添加到电子邮件
        /// </summary>
        /// <param name="attachments">要添加的附件列表</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail Attach(IList<Attachment> attachments);

        /// <summary>
        /// 同步发送电子邮件
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        SendResponse Send(CancellationToken? token = null);

        /// <summary>
        /// 异步发送电子邮件
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
	    Task<SendResponse> SendAsync(CancellationToken? token = null);

	    IFluentEmail AttachFromFilename(string filename,  string contentType = null, string attachmentName = null);

        /// <summary>
        /// 向电子邮件添加明文替代正文。 与HTML电子邮件结合使用，
        /// 这允许没有html功能的电子邮件阅读器，还有助于避免垃圾邮件过滤器。
        /// </summary>
        /// <param name="body">邮件正文</param>
        IFluentEmail PlaintextAlternativeBody(string body);

        /// <summary>
        /// 将模板添加到来自嵌入式资源的电子邮件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径嵌入式资源，例如[YourAssembly]。[YourResourceFolder]。[YourFilename.txt]</param>
        /// <param name="model">模板模型</param>
        /// <param name="assembly">资源所在的程序集。默认为调用程序集</param>
        /// <returns></returns>
        IFluentEmail PlaintextAlternativeUsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly);

        /// <summary>
        /// 将模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// <param name="model">模板模型</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail PlaintextAlternativeUsingTemplateFromFile<T>(string filename, T model);

        /// <summary>
        /// 将特定于文化的模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// /// <param name="model">剃刀模型</param>
        /// <param name="culture">模板的区域性（默认为当前区域性）</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail PlaintextAlternativeUsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture);

        /// <summary>
        /// 将剃刀模板添加到电子邮件
        /// </summary>
        /// <param name="template">剃刀模板</param>
        /// <param name="model">模板模型</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail PlaintextAlternativeUsingTemplate<T>(string template, T model);

        /// <summary>
        /// 将标签添加到电子邮件。 当前仅Mailgun提供商支持。 <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
        /// </summary>
        /// <param name="tag">标签名称，最多128个字符，仅ASCII</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail Tag(string tag);

        /// <summary>
        /// 将标题添加到电子邮件。
        /// </summary>
        /// <param name="header">标头名称，仅允许可打印的ASCII。</param>
        /// <param name="body">标头的值</param>
        /// <returns>Instance of the Email class</returns>
        IFluentEmail Header(string header, string body);
    }
}
