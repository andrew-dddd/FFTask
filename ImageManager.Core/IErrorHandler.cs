namespace ImageManager.Core
{
    public interface IErrorHandler
    {
        void ShowError(string message);

        void ShowUnexpectedError(string message);
    }
}
