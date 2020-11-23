using ImageManager.Core.Models;
using System.IO;
using System.Linq;

namespace ImageManager.Core.Impl
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
            CheckIfTargetDirExist(imageGallery.FullName);
            CheckIfIsGalleryDirectory(imageGallery.FullName);

            foreach (var galleryFile in imageGallery.GalleryFiles)
            {
                _fileManager.RemoveFile(galleryFile.FullName);
            }
            _fileManager.DestroyGallery(imageGallery.FullName);
        }

        public void InitializeGallery(string galleryDirectoryPath)
        {
            CheckIfTargetDirExist(galleryDirectoryPath);
            if (_fileManager.IsGalleryDirectory(galleryDirectoryPath)) throw new ImageManagerException("Already a gallery directory.");

            _fileManager.InitializeGallery(galleryDirectoryPath);
        }

        public ImageGallery OpenGallery(string galleryDirectoryPath)
        {
            CheckIfTargetDirExist(galleryDirectoryPath);
            CheckIfIsGalleryDirectory(galleryDirectoryPath);

            var galleryFiles = _fileManager.ReadGalleryFiles(galleryDirectoryPath)
                .Select(x => Create(galleryDirectoryPath, x.Key, x.Value))
                .ToList();

            return new ImageGallery(galleryDirectoryPath, galleryFiles);
        }

        public void RemoveImage(ImageGallery imageGallery, int currentImageIndex)
        {
            CheckIfTargetDirExist(imageGallery.FullName);
            CheckIfIsGalleryDirectory(imageGallery.FullName);
            if (imageGallery.Count == 0) throw new ImageManagerException("Gallery is empty");

            var currentImage = imageGallery.GalleryFiles[currentImageIndex];
            _fileManager.RemoveFile(currentImage.FullName);
            var existingImages = imageGallery.GalleryFiles.Where(x => x != currentImage).ToList();
            _fileManager.WriteGalleryFile(imageGallery.FullName, existingImages);
        }

        public void AddImage(ImageGallery imageGallery, NewImageInfo newImageInfo)
        {
            CheckIfTargetDirExist(imageGallery.FullName);
            CheckIfIsGalleryDirectory(imageGallery.FullName);

            var existingFileList = imageGallery.GalleryFiles.ToList();
            var newGalleryFileInfo = CreateGalleryFileInfo(newImageInfo);
            existingFileList.Add(newGalleryFileInfo);
            _fileManager.CopyFileToGallery(imageGallery.FullName, newGalleryFileInfo);
            _fileManager.WriteGalleryFile(imageGallery.FullName, existingFileList);
        }

        internal virtual void CheckIfTargetDirExist(string galleryDirectoryPath)
        {
            if (!_fileManager.DirectoryExists(galleryDirectoryPath)) throw new ImageManagerException($"Directory '{galleryDirectoryPath.Split('\\').LastOrDefault()}' does not exist");
        }

        internal virtual void CheckIfIsGalleryDirectory(string galleryDirectoryPath)
        {
            if (!_fileManager.IsGalleryDirectory(galleryDirectoryPath)) throw new ImageManagerException($"Directory '{galleryDirectoryPath.Split('\\').LastOrDefault()}' is not gallery directory");
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
