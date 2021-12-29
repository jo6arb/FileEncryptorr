using System;
using Microsoft.Extensions.Hosting;

namespace FileEncryptor.WPF
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] agrs) =>
            Host.CreateDefaultBuilder(agrs)
               .ConfigureServices(App.ConfigureServices);
    }
}