using ImageManager.Core;
using System;
using System.Windows;

namespace ImageManager.Impl
{
    public class ErrorHandler : IErrorHandler
    {
        public void ShowError(Exception exception)
        {
            MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowUnexpectedError(Exception exception)
        {
            MessageBox.Show(exception.Message, "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
