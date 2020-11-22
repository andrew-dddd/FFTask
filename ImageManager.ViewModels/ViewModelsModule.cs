using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("ImageManager.ViewModels.Tests")]

namespace ImageManager.ViewModels
{
    public static class ViewModelsModule
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddScoped<ImageViewerViewModel>();
            services.AddScoped<Services.IGalleryManager, Services.Impl.GalleryManager>();
            return services;
        }
    }
}
