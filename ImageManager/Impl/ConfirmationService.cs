using ImageManager.Core;
using System.Windows;

namespace ImageManager.Impl
{
    public class ConfirmationService : IConfirmationService
    {
        public bool Confirm(string message)
        {
            var result = MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}
