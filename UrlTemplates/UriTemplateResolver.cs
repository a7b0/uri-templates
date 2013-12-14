namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;

    public sealed class UriTemplateResolver
    {
        private readonly UriTemplate template;
        private readonly Dictionary<string, object> values;

        internal UriTemplateResolver(UriTemplate template)
        {
            this.template = template;
            this.values = new Dictionary<string, object>();
        }

        public string Resolve()
        {
            return template.Resolve(values);
        }

        public Uri ResolveUri()
        {
            return template.ResolveUri(values);
        }

        public UriTemplateResolver Bind(string name, string value)
        {
            return BindVariable(name, value);
        }

        public UriTemplateResolver Bind(string name, IEnumerable<string> values)
        {
            return BindVariable(name, values);
        }

        public UriTemplateResolver Bind(string name, IDictionary<string, string> values)
        {
            return BindVariable(name, values);
        }

        private UriTemplateResolver BindVariable(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            values.Add(name, value);
            return this;
        }
    }
}