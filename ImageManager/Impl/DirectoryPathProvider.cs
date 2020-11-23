using ImageManager.Core;

namespace ImageManager.Impl
{
    public class DirectoryPathProvider : IDirectoryPathProvider
    {
        public string GetDirectoryPath()
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }

            return null;
        }
    }
}
