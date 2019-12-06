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
    /// ����ʵ��
    /// </summary>
    public class Email : IFluentEmail
    {
        public EmailData Data { get; set; }
        public ITemplateRenderer Renderer { get; set; }
        public ISender Sender { get; set; }

        public static ITemplateRenderer DefaultRenderer = new ReplaceRenderer();
        public static ISender DefaultSender = new SaveToDiskSender("/");

        /// <summary>
        /// ʹ��Ĭ�����ô���һ���µĵ����ʼ�ʵ����
        /// </summary>
        /// <param name="client">Ҫ���͵�Smtp�ͻ���</param>
        public Email() : this(DefaultRenderer, DefaultSender) { }

        /// <summary>
        /// ����һ���µĵ����ʼ�ʵ�������а������ֺͷ�������������
        /// </summary>
        /// <param name="renderer">ģ����Ⱦ����</param>
        /// <param name="sender">�����ʼ�����ʵʩ</param>
        public Email(ITemplateRenderer renderer, ISender sender)
            : this(renderer, sender, null, null)
        {
        }

        /// <summary>
        ///  ���ض����ʼĵ�ַ��������Ĭ�����õ��µ����ʼ�ʵ����
        /// </summary>
        /// <param name="emailAddress">Ҫ���͵ĵ����ʼ���ַ</param>
        /// <param name="name">����������</param>
        public Email(string emailAddress, string name = "") 
            : this(DefaultRenderer, DefaultSender, emailAddress, name) { }

        /// <summary>
        ///  ʹ�ø�����������ʼ���ַ����һ���µ�Emailʵ����
        /// </summary>
        /// <param name="renderer">ģ����Ⱦ����</param>
        /// <param name="sender">�����ʼ�����ʵʩ</param>
        /// <param name="emailAddress">Ҫ���͵ĵ����ʼ���ַ</param>
        /// <param name="name">����������</param>
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
        /// ����һ���µ�Emailʵ��������from����
        /// </summary>
        /// <param name="emailAddress">Ҫ���͵ĵ����ʼ���ַ</param>
        /// <param name="name">����������</param>
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
        /// ���÷����Ե����ʼ���ַ
        /// </summary>
        /// <param name="emailAddress">�����˵ĵ����ʼ���ַ</param>
        /// <param name="name">����������</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail SetFrom(string emailAddress, string name = null)
        {
            Data.FromAddress = new Address(emailAddress, name ?? "");
            return this;
        }

        /// <summary>
        /// ���ռ�����ӵ������ʼ��У��ڡ�;���ϲ�����ƺ͵�ַ
        /// </summary>
        /// <param name="emailAddress">�ռ��˵ĵ����ʼ���ַ</param>
        /// <param name="name">�ռ�������</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail To(string emailAddress, string name = null)
        {
            if (emailAddress.Contains(";"))
            {
                //�����ʼ���ַ�����ֺţ��볢�Բ��
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
        /// ���ռ�����ӵ������ʼ�
        /// </summary>
        /// <param name="emailAddress">�ռ��˵ĵ����ʼ���ַ (�����';'���ж�ηָ�)</param>
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
        /// ���б��е������ռ�����ӵ������ʼ�
        /// </summary>
        /// <param name="mailAddresses">�ռ�������</param>
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
        /// ��������ӵ������ʼ�
        /// </summary>
        /// <param name="emailAddress">���͵����ʼ���ַ</param>
        /// <param name="name">��������</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail CC(string emailAddress, string name = "")
        {
            Data.CcAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// ���б��е����и�����ӵ������ʼ���
        /// </summary>
        /// <param name="mailAddresses">���͵��ռ����б�</param>
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
        /// �ڵ����ʼ�������ܼ�����
        /// </summary>
        /// <param name="emailAddress">�ܼ����͵ĵ����ʼ���ַ</param>
        /// <param name="name">�ܼ���������</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail BCC(string emailAddress, string name = "")
        {
            Data.BccAddresses.Add(new Address(emailAddress, name));
            return this;
        }

        /// <summary>
        /// ���б��е������ܼ����͸�����ӵ������ʼ���
        /// </summary>
        /// <param name="mailAddresses">�ܼ������ռ����б�</param>
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
        /// �ڵ����ʼ������ûظ���ַ
        /// </summary>
        /// <param name="address">�ظ���ַ</param>
        /// <returns></returns>
        public IFluentEmail ReplyTo(string address)
        {
            Data.ReplyToAddresses.Add(new Address(address));

            return this;
        }

        /// <summary>
        /// �ڵ����ʼ������ûظ���ַ
        /// </summary>
        /// <param name="address">�ظ���ַ</param>
        /// <param name="name">�ظ�����ʾ����</param>
        /// <returns></returns>
        public IFluentEmail ReplyTo(string address, string name)
        {
            Data.ReplyToAddresses.Add(new Address(address, name));

            return this;
        }

        /// <summary>
        /// ���õ����ʼ�������
        /// </summary>
        /// <param name="subject">�����ʼ�����</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail Subject(string subject)
        {
            Data.Subject = subject;
            return this;
        }

        /// <summary>
        /// ��������ӵ������ʼ�
        /// </summary>
        /// <param name="body">�ʼ�����</param>
        /// <param name="isHtml">�������ΪHTML����Ϊtrue�����ڴ��ı���Ϊfalse��Ĭ��ֵ��</param>
        public IFluentEmail Body(string body, bool isHtml = false)
        {
            Data.IsHtml = isHtml;
            Data.Body = body;
            return this;
        }

        /// <summary>
        /// ������ʼ��������������ġ� ��HTML�����ʼ����ʹ�ã�
        /// ������û��html���ܵĵ����ʼ��Ķ������������ڱ��������ʼ���������
        /// </summary>
        /// <param name="body">�ʼ�����</param>
        public IFluentEmail PlaintextAlternativeBody(string body)
        {
            Data.PlaintextAlternativeBody = body;
            return this;
        }

        /// <summary>
        /// �������ʼ����Ϊ�����ȼ�
        /// </summary>
        public IFluentEmail HighPriority()
        {
            Data.Priority = Priority.High;
            return this;
        }

        /// <summary>
        /// �������ʼ����Ϊ�����ȼ�
        /// </summary>
        public IFluentEmail LowPriority()
        {
            Data.Priority = Priority.Low;
            return this;
        }

        /// <summary>
        /// ����Ҫʹ�õ�ģ����Ⱦ���棬Ĭ��ΪRazorEngine
        /// </summary>
        public IFluentEmail UsingTemplateEngine(ITemplateRenderer renderer)
        {
            Renderer = renderer;
            return this;
        }

        /// <summary>
        /// ��ģ����ӵ�����Ƕ��ʽ��Դ�ĵ����ʼ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">·��Ƕ��ʽ��Դ������[YourAssembly]��[YourResourceFolder]��[YourFilename.txt]</param>
        /// <param name="model">ģ��ģ��</param>
        /// <param name="assembly">��Դ���ڵĳ��򼯡�Ĭ��Ϊ���ó��򼯡�</param>
        /// <param name="isHtml">�������ΪHTML��Ĭ��ֵ������Ϊtrue�����ڴ��ı���Ϊfalse</param>
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
        /// ��ģ����ӵ�����Ƕ��ʽ��Դ�ĵ����ʼ���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">·��Ƕ��ʽ��Դ������[YourAssembly]��[YourResourceFolder]��[YourFilename.txt]</param>
        /// <param name="model">ģ��ģ��</param>
        /// <param name="assembly">��Դ���ڵĳ��򼯡�Ĭ��Ϊ���ó��򼯡�</param>
        /// <returns></returns>
        public IFluentEmail PlaintextAlternativeUsingTemplateFromEmbedded<T>(string path, T model, Assembly assembly)
        {
            var template = EmbeddedResourceHelper.GetResourceAsString(assembly, path);
            var result = Renderer.Parse(template, model, false);
            Data.PlaintextAlternativeBody = result;

            return this;
        }


        /// <summary>
        /// ��ģ���ļ���ӵ������ʼ���
        /// </summary>
        /// <param name="filename">Ҫ���ص��ļ���·��</param>
        /// <param name="model">ģ��ģ��</param>
        /// <param name="isHtml">�������ΪHTML��Ĭ��ֵ������Ϊtrue�����ڴ��ı���Ϊfalse</param>
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
        /// ��ģ���ļ���ӵ������ʼ���
        /// </summary>
        /// <param name="filename">Ҫ���ص��ļ���·��</param>
        /// <param name="model">ģ��ģ��</param>
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
        /// ���ض����Ļ���ģ���ļ���ӵ������ʼ���
        /// </summary>
        /// <param name="filename">Ҫ���ص��ļ���·��</param>
        /// /// <param name="model">�굶ģ��</param>
        /// <param name="culture">ģ��������ԣ�Ĭ��Ϊ��ǰ�����ԣ�</param>
        /// <param name="isHtml">�������ΪHTML��Ĭ��ֵ������Ϊtrue�����ڴ��ı���Ϊfalse</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail UsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture, bool isHtml = true)
        {
            var cultureFile = GetCultureFileName(filename, culture);
            return UsingTemplateFromFile(cultureFile, model, isHtml);
        }

        /// <summary>
        /// ���ض����Ļ���ģ���ļ���ӵ������ʼ���
        /// </summary>
        /// <param name="filename">Ҫ���ص��ļ���·��</param>
        /// /// <param name="model">�굶ģ��</param>
        /// <param name="culture">ģ��������ԣ�Ĭ��Ϊ��ǰ�����ԣ�</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail PlaintextAlternativeUsingCultureTemplateFromFile<T>(string filename, T model, CultureInfo culture)
        {
            var cultureFile = GetCultureFileName(filename, culture);
            return PlaintextAlternativeUsingTemplateFromFile(cultureFile, model);
        }

        /// <summary>
        /// ���굶ģ����ӵ������ʼ�
        /// </summary>
        /// <param name="template">�굶ģ��</param>
        /// <param name="model">ģ��ģ��</param>
        /// <param name="isHtml">�������ΪHTML����Ϊtrue�����ڴ��ı���Ϊfalse����ѡ��</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail UsingTemplate<T>(string template, T model, bool isHtml = true)
        {
            var result = Renderer.Parse(template, model, isHtml);
            Data.IsHtml = isHtml;
            Data.Body = result;

            return this;
        }

        /// <summary>
        /// ���굶ģ����ӵ������ʼ�
        /// </summary>
        /// <param name="template">�굶ģ��</param>
        /// <param name="model">ģ��ģ��</param>
        /// <returns>Instance of the Email class</returns>
        public IFluentEmail PlaintextAlternativeUsingTemplate<T>(string template, T model)
        {
            var result = Renderer.Parse(template, model, false);
            Data.PlaintextAlternativeBody = result;

            return this;
        }

        /// <summary>
        /// ��������ӵ������ʼ�
        /// </summary>
        /// <param name="attachment">�������</param>
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
        /// �����������ӵ������ʼ�
        /// </summary>
        /// <param name="attachments">Ҫ��ӵĸ����б�</param>
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
        /// ����ǩ��ӵ������ʼ��� ��ǰ��Mailgun�ṩ��֧�֡� <see href="https://documentation.mailgun.com/en/latest/user_manual.html#tagging"/>
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
        /// ͬ�����͵����ʼ�
        /// </summary>
        /// <returns>Instance of the Email class</returns>
        public virtual SendResponse Send(CancellationToken? token = null)
        {
            return Sender.Send(this, token);
        }
        /// <summary>
        /// �첽���͵����ʼ�
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
