using System;
using System.Collections.Generic;

namespace Resta.UriTemplates
{
    public sealed class UriTemplateResolver
    {
        private readonly UriTemplate _template;
        private readonly Dictionary<string, object> _variables;

        internal UriTemplateResolver(UriTemplate template)
        {
            _template = template;
            _variables = new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public string Resolve() => _template.Resolve(_variables);

        public Uri ResolveUri() => _template.ResolveUri(_variables);

        public UriTemplate ResolveTemplate() => _template.ResolveTemplate(_variables);

        public UriTemplateResolver Bind(string name, string value) => BindVariable(name, value);

        public UriTemplateResolver Bind(string name, IEnumerable<string> values) => BindVariable(name, values);

        public UriTemplateResolver Bind(string name, IDictionary<string, string> values) => BindVariable(name, values);

        public UriTemplateResolver Ignore(string name) => BindVariable(name, null);

        public UriTemplateResolver Clear()
        {
            _variables.Clear();
            return this;
        }

        private UriTemplateResolver BindVariable(string name, object value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _variables.Add(name, value);
            return this;
        }
    }
}