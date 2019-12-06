namespace FluentEmail.Core
{
    /// <summary>
    /// 流利电子邮件工厂
    /// </summary>
    public interface IFluentEmailFactory
    {
        IFluentEmail Create();
    }
}