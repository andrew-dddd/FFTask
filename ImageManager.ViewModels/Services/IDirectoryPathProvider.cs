using System;
using System.Collections.Generic;
using System.Text;

namespace ImageManager.ViewModels.Services
{
    public interface IDirectoryPathProvider
    {
        string GetDirectoryPath();
    }

    public interface IErrorHandler
    {
        void ShowError(string message);

        void ShowUnexpectedError(string message);
    }
}
