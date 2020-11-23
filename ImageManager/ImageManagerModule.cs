using ImageManager.Core;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("ImageManager.Core.Tests")]
[assembly: InternalsVisibleTo("ImageManager.Tests")]
namespace ImageManager
{
    public static class ImageManagerModule
    {
        public static IServiceCollection AddImageManagerCommons(this IServiceCollection services)
        {
            services.AddScoped<IErrorHandler, Impl.ErrorHandler>();
            services.AddScoped<IDirectoryPathProvider, Impl.DirectoryPathProvider>();
            services.AddScoped<IConfirmationService, Impl.ConfirmationService>();
            services.AddScoped<INewImageDataProvider, Impl.NewImageDataProvider>();
            services.AddScoped<ImageViewerViewModel>();
            services.AddScoped<MainWindow>();
            return services;
        }
    }
}
