using ImageManager.Core;
using System.Windows;

namespace ImageManager.Impl
{
    public class ErrorHandler : IErrorHandler
    {
        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowUnexpectedError(string message)
        {
            MessageBox.Show(message, "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
