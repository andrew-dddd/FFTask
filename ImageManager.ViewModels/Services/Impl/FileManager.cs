using ImageManager.ViewModels.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageManager.ViewModels.Services.Impl
{
    internal class FileManager : IFileManager
    {
        private const string CreditsJsonFileName = "__gallery.json";

        public void InitializeGallery(string galleryDirectoryPath)
        {
            var galleryDirectory = new DirectoryInfo(galleryDirectoryPath);
            if (!galleryDirectory.Exists) throw new ImageManagerException("Directory does not exist.");
            File.WriteAllText(Path.Combine(galleryDirectoryPath, CreditsJsonFileName), "{}");
        }

        public void CopyFileToGallery(string galleryDirectoryPath, GalleryImageInfo newGalleryFileInfo)
        {
            var sourceFile = newGalleryFileInfo.FullName;
            var targetFile = Path.Combine(galleryDirectoryPath, Path.GetFileName(newGalleryFileInfo.FullName));
            File.Copy(sourceFile, targetFile, true);
        }

        public Dictionary<string, CreditsEntry> ReadGalleryFiles(string galleryDirectoryPath)
        {
            var creditsFile = new FileInfo(GetGalleryJsonFile(galleryDirectoryPath));
            var creditsFileJsonString = File.ReadAllText(creditsFile.FullName);
            return JsonConvert.DeserializeObject<Dictionary<string, CreditsEntry>>(creditsFileJsonString);
        }

        public void RemoveFile(string removedFileName)
        {
            if (removedFileName.EndsWith(CreditsJsonFileName)) throw new ImageManagerException("This file can not be removed.");
            File.Delete(removedFileName);
        }

        public void WriteGalleryFile(string galleryDirectoryPath, List<GalleryImageInfo> existingImages)
        {
            var creditsPath = GetGalleryJsonFile(galleryDirectoryPath);
            if (existingImages.Count == 0)
            {
                File.WriteAllText(creditsPath, "{}");
                return;
            }

            var creditsDictionary = existingImages.ToDictionary(x => x.OriginalFileName, x => x.FileCredits);
            var creditsJson = JsonConvert.SerializeObject(creditsDictionary, Formatting.Indented);
            File.WriteAllText(creditsPath, creditsJson);
        }

        public void DestroyGallery(string galleryDirectoryPath) => File.Delete(Path.Combine(galleryDirectoryPath, CreditsJsonFileName));

        public bool IsGalleryDirectory(string galleryDirectoryPath)
        {
            return File.Exists(GetGalleryJsonFile(galleryDirectoryPath));
        }

        public bool DirectoryExists(string directory) => Directory.Exists(directory);

        private static string GetGalleryJsonFile(string galleryDirectoryPath) => Path.Combine(galleryDirectoryPath, CreditsJsonFileName);
    }
}
