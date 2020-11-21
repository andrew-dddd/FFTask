using ImageManager.ViewModels.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageManager
{
    public static class ImageManagerModule
    {
        public static IServiceCollection AddImageManagerCommons(this IServiceCollection services)
        {
            services.AddScoped<IErrorHandler, Impl.ErrorHandler>();
            services.AddScoped<IDirectoryPathProvider, Impl.DirectoryPathProvider>();
            services.AddScoped<MainWindow>();
            return services;
        }
    }
}
