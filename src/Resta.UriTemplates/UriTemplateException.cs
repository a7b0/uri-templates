using System;

namespace Resta.UriTemplates
{
    public class UriTemplateException : Exception
    {
        public UriTemplateException(string message)
            : base(message)
        {
        }

        public UriTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}