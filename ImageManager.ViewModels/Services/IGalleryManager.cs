using ImageManager.ViewModels.Models;

namespace ImageManager.ViewModels.Services
{
    public interface IGalleryManager
    {
        void WipeGallery(ImageGallery imageGallery);
        void InitializeGallery(string galleryDirectoryPath);
        ImageGallery OpenGallery(string galleryDirectoryPath);
        void RemoveImage(ImageGallery imageGallery, int currentImageIndex);
        void AddImage(ImageGallery imageGallery, NewImageInfo newImageInfo);
    }
}
