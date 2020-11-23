using System;
using System.Runtime.Serialization;

namespace ImageManager.Core
{
    public sealed class ImageManagerException : Exception
    {
        public ImageManagerException()
        {
        }

        public ImageManagerException(string message) : base(message)
        {
        }

        public ImageManagerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ImageManagerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
