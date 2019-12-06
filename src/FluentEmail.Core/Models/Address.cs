namespace FluentEmail.Core.Models
{
    /// <summary>
    /// 地址
    /// </summary>
    public class Address
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Email地址
        /// </summary>
        public string EmailAddress { get; set; }

        public Address()
        {            
        }
        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="emailAddress">email地址</param>
        /// <param name="name">名称</param>
        public Address(string emailAddress, string name = null)
        {
            EmailAddress = emailAddress;
            Name = name;
        }
    }
}
