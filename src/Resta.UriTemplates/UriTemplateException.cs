namespace Resta.UriTemplates
{
    using System;

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