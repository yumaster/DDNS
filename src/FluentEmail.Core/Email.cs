using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentEmail.Core.Defaults;
using FluentEmail.Core.Interfaces;
using FluentEmail.Core.Models;

namespace FluentEmail.Core
{
    /// <summary>
    /// 邮箱实体
    /// </summary>
    public class Email : IFluentEmail
    {
        public EmailData Data { get; set; }
        public ITemplateRenderer Renderer { get; set; }
        public ISender Sender { get; set; }

        public static ITemplateRenderer DefaultRenderer = new ReplaceRenderer();
        public static ISender DefaultSender = new SaveToDiskSender("/");

        /// <summary>
        /// 使用默认设置创建一个新的电子邮件实例。
        /// </summary>
        /// <param name="client">要发送的Smtp客户端</param>
        public Email() : this(DefaultRenderer, DefaultSender) { }

        /// <summary>
        /// 创建一个新的电子邮件实例，其中包含呈现和发送引擎的替代。
        /// </summary>
        /// <param name="renderer">模板渲染引擎</param>
        /// <param name="sender">电子邮件发送实施</param>
        public Email(ITemplateRenderer renderer, ISender sender)
            : this(renderer, sender, null, null)
        {
        }

        /// <summary>
        ///  从特定的邮寄地址创建具有默认设置的新电子邮件实例。
        /// </summary>
        /// <param name="emailAddress">要发送的电子邮件地址</param>
        /// <param name="name">发件人姓名</param>
        public Email(string emailAddress, string name = "") 
            : this(DefaultRenderer, DefaultSender, emailAddress, name) { }

        /// <summary>
        ///  使用给定的引擎和邮件地址创建一个新的Email实例。
        /// </summary>
        /// <param name="renderer">模板渲染引擎</param>
        /// <param name="sender">电子邮件发送实施</param>
        /// <param name="emailAddress">要发送的电子邮件地址</param>
        /// <param name="name">发件人姓名</param>
        public Email(ITemplateRenderer renderer, ISender sender, string emailAddress, string name = "")
        {
            Data = new EmailData()
            {
                FromAddress = new Address() {EmailAddress = emailAddress, Name = name}
            };
            Renderer = renderer;
            Sender = sender;
        }

        /// <summary>
        /// 创建一个新的Email实例并设置from属性
        /// </summary>
        /// <param name="emailAddress">要发送的电子邮件地址</param>
        /// <param name="name">发件人姓名</param>
        /// <returns>Instance of the Email class</returns>
        public static IFluentEmail From(string emailAddress, string name = null)
        {
            var email = new Email
            {
                Data = {FromAddress = new Address(emailAddress, name ?? "")}
            };

            return email;
        }

        /// <summary>
        /// 设置发送自电子邮件地址
        /// </summary>
        /// <param name="emailAddress">发件人的电子邮件地址</param>
        /// <param name="name">发件人姓名</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail SetFrom(string emailAddress, string name = null)
        {
            Data.FromAddress = new Address(emailAddress, name ?? "");
            return this;
        }

        /// <summary>
        /// 将收件人添加到电子邮件中，在“;”上拆分名称和地址
        /// </summary>
        /// <param name="emailAddress">收件人的电子邮件地址</param>
        /// <param name="name">收件人姓名</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail To(string emailAddress, string name = null)
        {
            if (emailAddress.Contains(";"))
            {
                //电子邮件地址包含分号，请尝试拆分
                var nameSplit = name?.Split(';') ?? new string [0];
                var addressSplit = emailAddress.Split(';');
                for (int i = 0; i < addressSplit.Length; i++)
                {
                    var currentName = string.Empty;
                    if ((nameSplit.Length - 1) >= i)
                    {
                        currentName = nameSplit[i];
                    }
                    Data.ToAddresses.Add(new Address(addressSplit[i].Trim(), currentName.Trim()));
                }
            }
            else
            {
                Data.ToAddresses.Add(new Address(emailAddress.Trim(), name?.Trim()));
            }
            return this;
        }

        /// <summary>
        /// 将收件人添加到电子邮件
        /// </summary>
        /// <param name="emailAddress">收件人的电子邮件地址 (允许对';'进行多次分割)</param>
        /// <returns></returns>
        public IFluentEmail To(string emailAddress)
        {
            if (emailAddress.Contains(";"))
            {
                foreach (string address in emailAddress.Split(';'))
                {
                    Data.ToAddresses.Add(new Address(address));
                }
            }
            else
            {
                Data.ToAddresses.Add(new Address(emailAddress));
            }

            return this;
        }

        /// <summary>
        /// 将列表中的所有收件人添加到电子邮件
        /// </summary>
        /// <param name="mailAddresses">收件人名单</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail To(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Data.ToAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// 将复本添加到电子邮件
        /// </summary>
        /// <param name="emailAddress">抄送电子邮件地址</param>
        /// <param name="name">抄送名称</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail CC(string emailAddress, string name = "")
        {
            Data.CcAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// 将列表中的所有复本添加到电子邮件中
        /// </summary>
        /// <param name="mailAddresses">抄送的收件人列表</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail CC(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Data.CcAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// 在电子邮件中添加密件抄送
        /// </summary>
        /// <param name="emailAddress">密件抄送的电子邮件地址</param>
        /// <param name="name">密件抄送名称</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail BCC(string emailAddress, string name = "")
        {
            Data.BccAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// 将列表中的所有密件抄送副本添加到电子邮件中
        /// </summary>
        /// <param name="mailAddresses">密件抄送收件人列表</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail BCC(IList<Address> mailAddresses)
        {
            foreach (var address in mailAddresses)
            {
                Data.BccAddresses.Add(address);
            }
            return this;
        }

        /// <summary>
        /// 在电子邮件上设置回复地址
        /// </summary>
        /// <param name="address">回复地址</param>
        /// <returns></returns>
        public IFluentEmail ReplyTo(string address)
        {
            Data.ReplyToAddresses.Add(new Address(address));

            return this;
        }

        /// <summary>
        /// 在电子邮件上设置回复地址
        /// </summary>
        /// <param name="address">回复地址</param>
        /// <param name="name">回复的显示名称</param>
        /// <returns></returns>
        public IFluentEmail ReplyTo(string address, string name)
        {
            Data.ReplyToAddresses.Add(new Address(address, name));

            return this;
        }

        /// <summary>
        /// 设置电子邮件的主题
        /// </summary>
        /// <param name="subject">电子邮件主题</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail Subject(string subject)
        {
            Data.Subject = subject;
            return this;
        }

        /// <summary>
        /// 将正文添加到电子邮件
        /// </summary>
        /// <param name="body">邮件内容</param>
        /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（默认值）</param>
        public IFluentEmail Body(string body, bool isHtml = false)
        {
            Data.IsHtml = isHtml;
            Data.Body = body;
            return this;
        }

        /// <summary>
        /// 向电子邮件添加明文替代正文。 与HTML电子邮件结合使用，
        /// 这允许没有html功能的电子邮件阅读器，还有助于避免垃圾邮件过滤器。
        /// </summary>
        /// <param name="body">邮件内容</param>
        public IFluentEmail PlaintextAlternativeBody(string body)
        {
            Data.PlaintextAlternativeBody = body;
            return this;
        }

        /// <summary>
        /// 将电子邮件标记为高优先级
        /// </summary>
        public IFluentEmail HighPriority()
        {
            Data.Priority = Priority.High;
            return this;
        }

        /// <summary>
        /// 将电子邮件标记为低优先级
        /// </summary>
        public IFluentEmail LowPriority()
        {
            Data.Priority = Priority.Low;
            return this;
        }

        /// <summary>
        /// 设置要使用的模板渲染引擎，默认为RazorEngine
        /// </summary>
        public IFluentEmail UsingTemplateEngine(ITemplateRenderer renderer)
        {
            Renderer = renderer;
            return this;
        }

        /// <summary>
        /// 将模板添加到来自嵌入式资源的电子邮件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径嵌入式资源，例如[YourAssembly]。[YourResourceFolder]。[YourFilename.txt]</param>
        /// <param name="model">模板模型</param>
        /// <param name="assembly">资源所在的程序集。默认为调用程序集。</param>
        /// <param name="isHtml">如果主体为HTML（默认值），则为true；对于纯文本，为false</param>
        /// <returns></returns>
        public IFluentEmail UsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly, bool isHtml = true)
        {
            var template = EmbeddedResourceHelper.GetResourceAsString(assembly, path);
            var result = Renderer.Parse(template, model, isHtml);
            Data.IsHtml = isHtml;
            Data.Body = result;

            return this;
        }

        /// <summary>
        /// 将模板添加到来自嵌入式资源的电子邮件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">路径嵌入式资源，例如[YourAssembly]。[YourResourceFolder]。[YourFilename.txt]</param>
        /// <param name="model">模板模型</param>
        /// <param name="assembly">资源所在的程序集。默认为调用程序集。</param>
        /// <returns></returns>
        public IFluentEmail PlaintextAlternativeUsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly)
        {
            var template = EmbeddedResourceHelper.GetResourceAsString(assembly, path);
            var result = Renderer.Parse(template, model, false);
            Data.PlaintextAlternativeBody = result;

            return this;
        }


        /// <summary>
        /// 将模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// <param name="model">模板模型</param>
        /// <param name="isHtml">如果主体为HTML（默认值），则为true；对于纯文本，为false</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail UsingTemplateFromFile<T>(string filename, T model, bool isHtml = true)
        {
            var template = "";

            using (var reader = new StreamReader(File.OpenRead(filename)))
            {
                template = reader.ReadToEnd();
            }

            var result = Renderer.Parse(template, model, isHtml);
            Data.IsHtml = isHtml;
            Data.Body = result;

            return this;
        }

        /// <summary>
        /// 将模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// <param name="model">模板模型</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail PlaintextAlternativeUsingTemplateFromFile<T>(string filename, T model)
        {
            var template = "";

            using (var reader = new StreamReader(File.OpenRead(filename)))
            {
                template = reader.ReadToEnd();
            }

            var result = Renderer.Parse(template, model, false);
            Data.PlaintextAlternativeBody = result;

            return this;
        }

        /// <summary>
        /// 将特定于文化的模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// /// <param name="model">剃刀模型</param>
        /// <param name="culture">模板的区域性（默认为当前区域性）</param>
        /// <param name="isHtml">如果主体为HTML（默认值），则为true；对于纯文本，为false</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture, bool isHtml = true)
        {
            var cultureFile = GetCultureFileName(filename, culture);
            return UsingTemplateFromFile(cultureFile, model, isHtml);
        }

        /// <summary>
        /// 将特定于文化的模板文件添加到电子邮件中
        /// </summary>
        /// <param name="filename">要加载的文件的路径</param>
        /// /// <param name="model">剃刀模型</param>
        /// <param name="culture">模板的区域性（默认为当前区域性）</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail PlaintextAlternativeUsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture)
        {
            var cultureFile = GetCultureFileName(filename, culture);
            return PlaintextAlternativeUsingTemplateFromFile(cultureFile, model);
        }

        /// <summary>
        /// 将剃刀模板添加到电子邮件
        /// </summary>
        /// <param name="template">剃刀模板</param>
        /// <param name="model">模板模型</param>
        /// <param name="isHtml">如果主体为HTML，则为true；对于纯文本，为false（可选）</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail UsingTemplate<T>(string template, T model, bool isHtml = true)
        {
            var result = Renderer.Parse(template, model, isHtml);
            Data.IsHtml = isHtml;
            Data.Body = result;

            return this;
        }

        /// <summary>
        /// 将剃刀模板添加到电子邮件
        /// </summary>
        /// <param name="template">剃刀模板</param>
        /// <param name="model">模板模型</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail PlaintextAlternativeUsingTemplate<T>(string template, T model)
        {
            var result = Renderer.Parse(template, model, false);
            Data.PlaintextAlternativeBody = result;

            return this;
        }

        /// <summary>
        /// 将附件添加到电子邮件
        /// </summary>
        /// <param name="attachment">附件添加</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail Attach(Attachment attachment)
        {
            if (!Data.Attachments.Contains(attachment))
            {
                Data.Attachments.Add(attachment);
            }

            return this;
        }

        /// <summary>
        /// 将多个附件添加到电子邮件
        /// </summary>
        /// <param name="attachments">要添加的附件列表</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail Attach(IList<Attachment> attachments)
        {
            foreach (var attachment in attachments.Where(attachment => !Data.Attachments.Contains(attachment)))
            {
                Data.Attachments.Add(attachment);
            }
            return this;
        }

        public IFluentEmail AttachFromFilename(string filename,  string contentType = null, string attachmentName = null)
        {
            var stream = File.OpenRead(filename);
            Attach(new Attachment()
            {
                Data = stream,
                Filename = attachmentName ?? filename,
                ContentType = contentType
            });

            return this;
        }

        /// <summary>
        /// 将标签添加到电子邮件。 当前仅Mailgun提供商支持。 <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
        /// </summary>
        /// <param name="tag">Tag name, max 128 characters, ASCII only</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail Tag(string tag)
        {
            Data.Tags.Add(tag);

            return this;
        }

        public IFluentEmail Header(string header, string body)
        {
            Data.Headers.Add(header, body);

            return this;
        }

        /// <summary>
        /// 同步发送电子邮件
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public virtual SendResponse Send(CancellationToken? token = null)
        {
            return Sender.Send(this, token);
        }
        /// <summary>
        /// 异步发送电子邮件
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<SendResponse> SendAsync(CancellationToken? token = null)
        {
            return await Sender.SendAsync(this, token);
        }

        private static string GetCultureFileName(string fileName, CultureInfo culture)
        {
            var extension = Path.GetExtension(fileName);
            var cultureExtension = string.Format("{0}{1}", culture.Name, extension);

            var cultureFile = Path.ChangeExtension(fileName, cultureExtension);
            if (File.Exists(cultureFile))
                return cultureFile;
            else
                return fileName;
        }
    }
}
