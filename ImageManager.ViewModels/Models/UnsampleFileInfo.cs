using System.IO;

namespace ImageManager.ViewModels.Models
{
    public sealed class GalleryImageInfo
    {
        internal GalleryImageInfo(string fullName, bool exists, CreditsEntry fileCredits)
        {   
            FullName = fullName;
            Name = Path.GetFileName(fullName);
            Exists = exists;
            OriginalFileName = Path.GetFileNameWithoutExtension(fullName);
            FileCredits = fileCredits;
        }        

        /// <summary>
        /// Gets full file name (path).
        /// </summary>
        public string FullName { get; }

        /// <summary>
        /// Gets a file name with extension.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether file exists.
        /// </summary>
        public bool Exists { get; }

        /// <summary>
        /// Gets Name without extension.
        /// </summary>
        public string OriginalFileName { get; }

        /// <summary>
        /// File credits.
        /// </summary>
        public CreditsEntry FileCredits { get; }
    }
}
