using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using osVodigiPlayer.HostedService;
using osVodigiPlayer.Settings;
using osVodigiPlayer.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace osVodigiPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = CreateHostBuilder().Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args = null)
        {
            /*
            return new HostBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        ConfigureServices(services);
                    });
            */
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(context.Configuration, services);
                });
        }

        private static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<PlayerSettings>(configuration.GetSection(nameof(PlayerSettings)));

            //services.AddScoped<ISampleService, SampleService>();

            services.AddHostedService<HelloHostedService>();
            //services.AddSingleton<ILogBase>(new LogBase(new FileInfo($@"C:\temp\vodigipl.txt")));
            //services.AddSingleton<ITextService, TextService>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AdminWindow>();
        }

        protected async void OnStartup(object sender, StartupEventArgs e)
        {
            await _host.StartAsync();

            //var window = _host.Services.GetRequiredService<AdminWindow>();
            var window = _host.Services.GetService<MainWindow>();
            window.Show();

            base.OnStartup(e);
        }

        protected async void OnExit(object sender, ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }

            base.OnExit(e);
        }

        private void ConfigureHangFireServices(IServiceCollection services)
        {
            services.AddHangfire(x =>
                x.UseSimpleAssemblyNameTypeSerializer()
                .UseDefaultTypeSerializer()
                .UseMemoryStorage());
            //services.AddHostedService<BackgroundJobServerHostedService>();
            services.AddHangfireServer();
        }

        private void SetupSch()
        {
            string userName = "Paulo Borges";
            IBackgroundJobClient backgroundJobClient = _host.Services.GetService<IBackgroundJobClient>();
            backgroundJobClient.Enqueue(() => Console.WriteLine("Hello Hangfire job!"));

            var jobId = BackgroundJob.Schedule(() => SendInvoiceMail(userName), TimeSpan.FromMinutes(2));
            Console.WriteLine($"Job Id {jobId} Completed. Delayed Welcome Mail Sent!");

            RecurringJob.AddOrUpdate(() => SendInvoiceMail(userName), Cron.Minutely);
            Console.WriteLine($"Recurring Job Scheduled. Invoice will be mailed Monthly for {userName}!");
        }

        public void SendInvoiceMail(string userName)
        {
            //Logic to Mail the user
            Console.WriteLine($"Here is your invoice, {userName}");
        }
    }
}
