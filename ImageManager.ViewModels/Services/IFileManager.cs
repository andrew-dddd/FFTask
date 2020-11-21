﻿using ImageManager.ViewModels.Models;
using System.Collections.Generic;

namespace ImageManager.ViewModels.Services
{
    public interface IFileManager
    {
        void DestroyGallery(string galleryDirectoryPath);
        void InitializeGallery(string galleryDirectoryPath);
        void RemoveFile(string currentImageFullName);
        void WriteCreditsFile(string galleryDirectoryPath, List<GalleryImageInfo> existingImages);
        void CopyFileToGallery(string galleryDirectoryPath, GalleryImageInfo newGalleryFileInfo);
        Dictionary<string, CreditsEntry> ReadGalleryFiles(string galleryDirectoryPath);
        bool IsGalleryDirectory(string galleryDirectoryPath);
        bool DirectoryExists(string directory);
    }
}