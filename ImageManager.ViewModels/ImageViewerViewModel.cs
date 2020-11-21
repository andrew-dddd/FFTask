using GalaSoft.MvvmLight;
using ImageManager.ViewModels.Models;
using ImageManager.ViewModels.Services;
using System;

namespace ImageManager.ViewModels
{
    public class ImageViewerViewModel : ViewModelBase
    {
        private GalleryImageInfo _currentImage;
        private ImageGallery _imageGallery;
        private CreditsEntry _currentCredits;
        private string _currentFileName;
        private int _currentFileIndex;
        private string currentImageFullName = "";
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
            private set
            {
                _currentCredits = value;
                RaisePropertyChanged();
            }
        }

        public GalleryImageInfo CurrentImage
        {
            get => _currentImage;
            private set
            {
                _currentImage = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentImageName
        {
            get => _currentFileName;
            set
            {
                _currentFileName = value;
                RaisePropertyChanged();
            }
        }

        public string CurrentImageFullName
        {
            get => currentImageFullName;
            private set
            {
                currentImageFullName = value;
                RaisePropertyChanged();
            }
        }

        public int CurrentImageIndex
        {
            get => _currentFileIndex;
            private set
            {
                _currentFileIndex = value;
                RaisePropertyChanged();
            }
        }

        public bool GalleryLoaded
        {
            get => _galleryLoader;
            set
            {
                _galleryLoader = value;
                RaisePropertyChanged();
            }
        }

        public bool GalleryHasImages
        {
            get => _galleryHasImages;
            private set
            {
                _galleryHasImages = value;
                RaisePropertyChanged();
            }
        }

        public void FirstImage() => SetCurrentFile(0);

        public void PreviousImage() => SetCurrentFile(CurrentImageIndex - 1);

        public void NextImage() => SetCurrentFile(CurrentImageIndex + 1);

        public void LastImage() => SetCurrentFile(_imageGallery.Count - 1);

        public void InitializeGallery() => TryExecute(() =>
        {
            var path = _directoryPathProvider.GetDirectoryPath();
            if (string.IsNullOrEmpty(path) == false)
            {
                _galleryManager.InitializeGallery(path);
            }
        });

        public void WipeGallery() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            if (_confirmationService.Confirm("Are you sure you want to wipe gallery? This will remove all images permanently."))
            {
                _galleryManager.WipeGallery(_imageGallery);
            }
        });

        public void RemoveImage() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            if (_confirmationService.Confirm($"Are you sure you want to remove '{CurrentImageName}'? This operation is permanent."))
            {
                _galleryManager.RemoveFile(_imageGallery, CurrentImageIndex);
            }
        });

        public void AddImage() => TryExecute(() =>
        {
            if (!GalleryLoaded) throw new ImageManagerException("Open gallery first");
            var newImageInfo = _newImageDataProvider.GetImageInfo();
            if (newImageInfo != null)
            {
                _galleryManager.AddImage(_imageGallery, newImageInfo);
            }
        });

        public void OpenGallery() => TryExecute(() =>
        {
            var galleryDirectoryPath = _directoryPathProvider.GetDirectoryPath();
            _imageGallery = _galleryManager.OpenGallery(galleryDirectoryPath);
            GalleryLoaded = true;
            if (!_imageGallery.IsEmptyGallery)
            {
                FirstImage();
                GalleryHasImages = true;
            }
        });

        private void SetCurrentFile(int fileIndex) => TryExecute(() =>
        {
            if (_imageGallery == null) throw new ImageManagerException("No loaded gallery");
            if (GalleryHasImages == false) throw new ImageManagerException("Gallery is empty");

            if (fileIndex >= _imageGallery.Count) fileIndex = 0;
            if (fileIndex < 0) fileIndex = _imageGallery.Count - 1;

            var unsampleFile = _imageGallery.GalleryFiles[CurrentImageIndex];

            CurrentImageIndex = fileIndex;
            CurrentImageName = unsampleFile.Name;
            CurrentImage = unsampleFile;
            CurrentCredits = unsampleFile.FileCredits;
        });

        private void TryExecute(Action action)
        {
            try
            {
                action = action ?? throw new ArgumentNullException(nameof(action));
                action();
            }
            catch (ImageManagerException ex)
            {
                _errorHandler.ShowError(ex.Message);
            }
            catch (Exception e)
            {
                _errorHandler.ShowUnexpectedError(e.Message);
            }
        }
    }
}
