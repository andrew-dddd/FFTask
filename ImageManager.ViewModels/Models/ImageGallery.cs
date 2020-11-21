using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ImageManager.ViewModels.Models
{
    public class ImageGallery
    {
        internal ImageGallery(string fullName, List<GalleryImageInfo> gallerFiles)
        {
            FullName = fullName;
            Name = Path.GetDirectoryName(fullName);
            GalleryFiles = gallerFiles.AsReadOnly(); 
        }

        internal ImageGallery()
        {
        }

        /// <summary>
        /// Gets the full name of the directory.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets collection of retrieved files.
        /// </summary>
        public ReadOnlyCollection<GalleryImageInfo> GalleryFiles { get; }

        /// <summary>
        /// Gets directory file count;
        /// </summary>
        public int Count => (GalleryFiles?.Count).GetValueOrDefault();

        /// <summary>
        /// Gets a value indicating if current gallery has any images.
        /// </summary>
        public bool IsEmptyGallery => Count == 0;
    }
}
