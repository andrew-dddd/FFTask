using ImageManager.Core.Models;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace ImageManager
{
    /// <summary>
    /// Interaction logic for GetNewImageWindow.xaml
    /// </summary>
    public partial class GetNewImageWindow : Window
    {
        public GetNewImageWindow()
        {
            InitializeComponent();
        }

        public NewImageInfo CreateNewImageInfo()
        {
            return new NewImageInfo
            {
                FullFilePath = FullPathLabel.Content.ToString(),
                PhotoUrl = PhotoUrl_TextBox.Text,
                UserName = UserName_TextBox.Text,
                UserUrl = UserUrl_TextBox.Text,
            };
        }

        private void AddImage_Button_Click(object sender, RoutedEventArgs e)
        {
            if (FullPathLabel.Content == null || !File.Exists(FullPathLabel.Content.ToString())) { ShowMessage("Full path is required or file does not exist"); return; }
            if (string.IsNullOrEmpty(UserName_TextBox.Text)) { ShowMessage("User name is required"); return; }

            DialogResult = true;
        }

        private void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PickImage_Button_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JPEGs (*.jpg) |*.jpg",
                CheckFileExists = true,
                Multiselect = false
            };

            var dr = openFileDialog.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                FullPathLabel.Content = openFileDialog.FileName;
            }
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
