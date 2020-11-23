using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("ImageManager.Core.Tests")]
[assembly: InternalsVisibleTo("ImageManager.Tests")]
namespace ImageManager.Core
{
    public static class CoreModule
    {
        public static IServiceCollection AddImageManagerCore(this IServiceCollection services)
        {   
            services.AddScoped<IGalleryManager, Impl.GalleryManager>();
            services.AddScoped<IFileManager, Impl.FileManager>();
            return services;
        }
    }
}
