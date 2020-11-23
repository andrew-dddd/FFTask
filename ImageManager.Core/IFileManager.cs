using ImageManager.Core.Models;
using System.Collections.Generic;

namespace ImageManager.Core
{
    public interface IFileManager
    {
        void DestroyGallery(string galleryDirectoryPath);
        void InitializeGallery(string galleryDirectoryPath);
        void RemoveFile(string currentImageFullName);
        void WriteGalleryFile(string galleryDirectoryPath, List<GalleryImageInfo> existingImages);
        void CopyFileToGallery(string galleryDirectoryPath, GalleryImageInfo newGalleryFileInfo);
        Dictionary<string, CreditsEntry> ReadGalleryFiles(string galleryDirectoryPath);
        bool IsGalleryDirectory(string galleryDirectoryPath);
        bool DirectoryExists(string directory);
    }
}
