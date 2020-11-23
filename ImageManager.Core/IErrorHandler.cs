using System;

namespace ImageManager.Core
{
    public interface IErrorHandler
    {
        void ShowError(Exception exception);

        void ShowUnexpectedError(Exception exception);
    }
}
