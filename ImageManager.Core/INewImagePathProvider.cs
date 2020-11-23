using ImageManager.Core.Models;

namespace ImageManager.Core
{
    public interface INewImageDataProvider
    {
        NewImageInfo GetImageInfo();
    }
}
