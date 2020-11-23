using ImageManager.Core;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IErrorHandler errorHandler;

        public MainWindow(ImageViewerViewModel imageViewerViewModel, IErrorHandler errorHandler)
        {
            DataContext = imageViewerViewModel;
            imageViewerViewModel.PropertyChanged += ImageViewerViewModel_PropertyChanged;
            InitializeComponent();
            this.errorHandler = errorHandler;
        }

        private void ImageViewerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ImageViewerViewModel.CurrentImage))
            {
                SetImage();
            }
        }

        public ImageViewerViewModel ViewModel => DataContext as ImageViewerViewModel;

        internal void OpenImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.OpenGallery();

        internal void First_Click(object sender, RoutedEventArgs e) => ViewModel.FirstImage();

        internal void Previous_Click(object sender, RoutedEventArgs e) => ViewModel.PreviousImage();

        internal void Next_Click(object sender, RoutedEventArgs e) => ViewModel.NextImage();

        internal void Last_Click(object sender, RoutedEventArgs e) => ViewModel.LastImage();

        internal void AddImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.AddImage();

        internal void RemoveImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.RemoveImage();

        internal void CreateGallery_Button_Click(object sender, RoutedEventArgs e) => ViewModel.InitializeGallery();

        private void SetImage()
        {
            try
            {
                if (ViewModel.CurrentImage == null)
                {
                    ImageControl.Source = null;
                }
                else
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(ViewModel.CurrentImage.FullName);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    ImageControl.Source = image;
                }
            }
            catch (Exception ex)
            {
                errorHandler.ShowUnexpectedError(ex);
            }
        }

        internal void DestroyGallery_Button_Click(object sender, RoutedEventArgs e) => ViewModel.DestroyGallery();
    }
}
