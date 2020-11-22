using ImageManager.ViewModels.Models;
using System.IO;
using System.Linq;

namespace ImageManager.ViewModels.Services.Impl
{
    internal class GalleryManager : IGalleryManager
    {
        private readonly IFileManager _fileManager;

        public GalleryManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        public void WipeGallery(ImageGallery imageGallery)
        {
            foreach (var galleryFile in imageGallery.GalleryFiles)
            {
                _fileManager.RemoveFile(galleryFile.FullName);
            }
            _fileManager.DestroyGallery(imageGallery.FullName);
        }

        public void InitializeGallery(string galleryDirectoryPath)
        {
            if (!_fileManager.DirectoryExists(galleryDirectoryPath)) throw new ImageManagerException("Target directory does not exist");
            if (_fileManager.IsGalleryDirectory(galleryDirectoryPath)) throw new ImageManagerException("This is already gallery directory");
            _fileManager.InitializeGallery(galleryDirectoryPath);
        }

        public ImageGallery OpenGallery(string galleryDirectoryPath)
        {
            if (!_fileManager.DirectoryExists(galleryDirectoryPath)) throw new ImageManagerException("Target directory does not exist");
            if (!_fileManager.IsGalleryDirectory(galleryDirectoryPath)) throw new ImageManagerException("Not a gallery directory.");

            var unsampleFilesDictionary = _fileManager.ReadGalleryFiles(galleryDirectoryPath);

            var unsampleFiles = unsampleFilesDictionary
                .Select(x => Create(galleryDirectoryPath, x.Key, x.Value))
                .ToList();

            return new ImageGallery(galleryDirectoryPath, unsampleFiles);
        }

        public void RemoveFile(ImageGallery imageGallery, int currentImageIndex)
        {
            var currentImage = imageGallery.GalleryFiles[currentImageIndex];
            _fileManager.RemoveFile(currentImage.FullName);
            var existingImages = imageGallery.GalleryFiles.Where(x => x != currentImage).ToList();
            _fileManager.WriteGalleryFile(imageGallery.FullName, existingImages);
        }

        public void AddImage(ImageGallery imageGallery, NewImageInfo newImageInfo)
        {
            var existingFileList = imageGallery.GalleryFiles.ToList();
            var newGalleryFileInfo = CreateGalleryFileInfo(newImageInfo);
            existingFileList.Add(newGalleryFileInfo);
            _fileManager.CopyFileToGallery(imageGallery.FullName, newGalleryFileInfo);
            _fileManager.WriteGalleryFile(imageGallery.FullName, existingFileList);
        }

        private GalleryImageInfo CreateGalleryFileInfo(NewImageInfo newImageInfo)
        {
            var fileInfo = new FileInfo(newImageInfo.FullFilePath);
            return new GalleryImageInfo(
               fileInfo.FullName,
               fileInfo.Exists,
               new CreditsEntry
               {
                   PhotoUrl = newImageInfo.PhotoUrl,
                   UserName = newImageInfo.UserName,
                   UserUrl = newImageInfo.UserUrl,
               });
        }

        private static GalleryImageInfo Create(string galleryDirectoryPath, string fileName, CreditsEntry fileCredits)
        {
            var fileInfo = new FileInfo(Path.Combine(galleryDirectoryPath, $"{fileName}.jpg"));
            return new GalleryImageInfo(fileInfo.FullName, fileInfo.Exists, fileCredits);
        }
    }
}
