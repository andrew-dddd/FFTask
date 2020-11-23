using ImageManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace ImageManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; set; }
        private IServiceScope Scope { get; set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddImageManagerCommons();
            serviceCollection.AddImageManagerCore();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Scope = ServiceProvider.CreateScope();
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var window = Scope.ServiceProvider.GetRequiredService<MainWindow>();
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Scope.Dispose();
            base.OnExit(e);
        }
    }
}
