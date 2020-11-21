using ImageManager.ViewModels.Models;

namespace ImageManager.ViewModels.Services
{
    public interface INewImageDataProvider
    {
        NewImageInfo GetImageInfo();
    }
}
