namespace Resta.UriTemplates
{
    using System;

    public sealed class UriTemplateParseException : UriTemplateException
    {
        public UriTemplateParseException(string message, string template, int position)
            : base(message)
        {
            Position = position;
            Template = template;
        }

        public UriTemplateParseException(string message, string template, int position, Exception innerException)
            : base(message, innerException)
        {
            Position = position;
            Template = template;
        }

        public int Position { get; }

        public string Template { get; }
    }
}