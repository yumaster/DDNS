using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //services
            //    .AddFluentEmail("yumaster@yeah.net")
            //    .AddRazorRenderer()
            //    .AddSmtpSender("smtp.yeah.net", 25);
            services.AddFluentEmail("cn_zhangyu@yeah.net", "土伦").AddRazorRenderer()
                .AddSmtpSender(new System.Net.Mail.SmtpClient()
                {
                    Host = "smtp.yeah.net",
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Port = 25,
                    Credentials = new System.Net.NetworkCredential("cn_zhangyu@yeah.net", "zhangyu******")
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
