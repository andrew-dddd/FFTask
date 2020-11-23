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

        ImageViewerViewModel ViewModel => DataContext as ImageViewerViewModel;

        private void OpenImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.OpenGallery();

        private void First_Click(object sender, RoutedEventArgs e) => ViewModel.FirstImage();

        private void Previous_Click(object sender, RoutedEventArgs e) => ViewModel.PreviousImage();

        private void Next_Click(object sender, RoutedEventArgs e) => ViewModel.NextImage();

        private void Last_Click(object sender, RoutedEventArgs e) => ViewModel.LastImage();

        private void SetImage()
        {
            try
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(ViewModel.CurrentImage.FullName);
                image.EndInit();
                ImageControl.Source = image;
            }
            catch (Exception ex)
            {
                errorHandler.ShowUnexpectedError(ex.Message);
            }
        }

        private void AddImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.AddImage();

        private void RemoveImage_Button_Click(object sender, RoutedEventArgs e) => ViewModel.RemoveImage();
    }
}
