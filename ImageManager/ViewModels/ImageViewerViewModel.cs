using GalaSoft.MvvmLight;
using ImageManager.Core.Models;
using ImageManager.Core;
using System;

namespace ImageManager.Core
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private GalleryImageInfo _currentImage;
        private ImageGallery _imageGallery;
        private CreditsEntry _currentCredits;
        private string _currentFileName;
        private int _currentFileIndex;
        private bool _galleryLoader;
        private bool _galleryHasImages;
        private readonly IGalleryManager _galleryManager;
        private readonly IDirectoryPathProvider _directoryPathProvider;
        private readonly IErrorHandler _errorHandler;
        private readonly IConfirmationService _confirmationService;
        private readonly INewImageDataProvider _newImageDataProvider;

        public ImageViewerViewModel(
            IGalleryManager unsamplesDirectoryOpener,
            IDirectoryPathProvider directoryPathProvider,
            IErrorHandler errorHandler,
            IConfirmationService confirmationService,
            INewImageDataProvider newImageDataProvider)
        {
            _galleryManager = unsamplesDirectoryOpener;
            _directoryPathProvider = directoryPathProvider;
            _errorHandler = errorHandler;
            _confirmationService = confirmationService;
            _newImageDataProvider = newImageDataProvider;
        }

        public CreditsEntry CurrentCredits
        {
            get => _currentCredits;
            internal set
            {
                _currentCredits = value;
                RaisePropertyChanged();
            }
        }

        public GalleryImageInfo CurrentImage
        {
            get => _currentImage;
            internal set
            {
                _currentImage = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentImageName
        {
            get => _currentFileName;
            internal set
            {
                _currentFileName = value;
                RaisePropertyChanged();
            }
        }

        public int CurrentImageIndex
        {
            get => _currentFileIndex;
            internal set
            {
                _currentFileIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool GalleryLoaded
        {
            get => _galleryLoader;
            internal set
            {
                _galleryLoader = value;
                RaisePropertyChanged();
            }
        }

        public bool GalleryHasImages
        {
            get => _galleryHasImages;
            internal set
            {
                _galleryHasImages = value;
                RaisePropertyChanged();
            }
        }

        public ImageGallery ImageGallery
        {
            get => _imageGallery;
            internal set
            {
                _imageGallery = value;
                RaisePropertyChanged();
            }
        }

        public void FirstImage() => TryExecute(() => SetCurrentFile(0));

        public void PreviousImage() => TryExecute(() => SetCurrentFile(CurrentImageIndex - 1));

        public void NextImage() => TryExecute(() => SetCurrentFile(CurrentImageIndex + 1));

        public void LastImage() => TryExecute(() => SetCurrentFile(_imageGallery.Count - 1));

        public void InitializeGallery() => TryExecute(() =>
        {
            var path = _directoryPathProvider.GetDirectoryPath();
            if (string.IsNullOrEmpty(path) == false)
            {
                _galleryManager.InitializeGallery(path);
                var gallery = _galleryManager.OpenGallery(path);
                LoadGallery(gallery);
            }
        });

        public void DestroyGallery() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            if (_confirmationService.Confirm("Are you sure you want to wipe gallery? This will remove all images permanently."))
            {
                _galleryManager.WipeGallery(_imageGallery);
                GalleryLoaded = false;
                GalleryHasImages = false;
                ImageGallery = null;

                CurrentCredits = null;
                CurrentImage = null;
                CurrentImageName = null;
                CurrentImageIndex = 0;
            }
        });

        public void RemoveImage() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            if (_confirmationService.Confirm($"Are you sure you want to remove '{CurrentImageName}'? This operation is permanent."))
            {
                CurrentImage = null;
                _galleryManager.RemoveImage(_imageGallery, CurrentImageIndex);
                var gallery = _galleryManager.OpenGallery(_imageGallery.FullName);
                LoadGallery(gallery);
            }
        });

        public void AddImage() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            var newImageInfo = _newImageDataProvider.GetImageInfo();
            if (newImageInfo != null)
            {
                _galleryManager.AddImage(_imageGallery, newImageInfo);
                var gallery = _galleryManager.OpenGallery(_imageGallery.FullName);
                LoadGallery(gallery);
            }
        });

        public void OpenGallery() => TryExecute(() =>
        {
            var galleryDirectoryPath = _directoryPathProvider.GetDirectoryPath();
            if (galleryDirectoryPath != null)
            {
                var gallery = _galleryManager.OpenGallery(galleryDirectoryPath);
                LoadGallery(gallery);
            }
        });

        internal virtual void LoadGallery(ImageGallery imageGallery)
        {
            _imageGallery = imageGallery;
            GalleryLoaded = true;
            if (!_imageGallery.IsEmptyGallery)
            {
                GalleryHasImages = true;
                FirstImage();
            }
        }

        private void SetCurrentFile(int fileIndex)
        {
            if (_imageGallery == null) throw new ImageManagerException("No loaded gallery");
            if (GalleryHasImages == false) throw new ImageManagerException("Gallery is empty");

            if (fileIndex >= _imageGallery.Count) fileIndex = 0;
            if (fileIndex < 0) fileIndex = _imageGallery.Count - 1;

            var unsampleFile = _imageGallery.GalleryFiles[fileIndex];

            CurrentImageIndex = fileIndex;
            CurrentImageName = unsampleFile.Name;
            CurrentImage = unsampleFile;
            CurrentCredits = unsampleFile.Credits;
        }

        private void TryExecute(Action action)
        {
            try
            {
                action = action ?? throw new ArgumentNullException(nameof(action));
                action();
            }
            catch (ImageManagerException ex)
            {
                _errorHandler.ShowError(ex);
            }
            catch (Exception e)
            {
                _errorHandler.ShowUnexpectedError(e);
            }
        }
    }
}
