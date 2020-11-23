using ImageManager.Core;
using ImageManager.Core.Models;

namespace ImageManager.Impl
{
    public class NewImageDataProvider : INewImageDataProvider
    {
        public NewImageInfo GetImageInfo()
        {
            GetNewImageWindow window = new GetNewImageWindow();
            var dr = window.ShowDialog();
            return dr.GetValueOrDefault()
                ? window.CreateNewImageInfo()
                : null;
        }
    }

}
