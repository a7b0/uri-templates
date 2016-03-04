using System;
using System.Collections.Generic;
using System.Text;

namespace Resta.UriTemplates
{
    /// <summary>
    ///     URI template processor (RFC6570).
    /// </summary>
    public sealed class UriTemplate
    {
        private readonly List<IUriComponent> _components;
        private List<VarSpec> _variables;

        public UriTemplate(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            Template = template;
            _components = UriTemplateParser.Parse(template);
        }

        internal UriTemplate(IEnumerable<IUriComponent> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException(nameof(components));
            }

            _components = new List<IUriComponent>(components);
            Template = GetTemplateString(_components);
        }

        public string Template { get; }

        public IEnumerable<VarSpec> Variables
        {
            get
            {
                if (_variables == null)
                {
                    _variables = GetVariables(_components);
                }

                return _variables;
            }
        }

        internal IEnumerable<IUriComponent> Components => _components;

        public UriTemplateResolver GetResolver()
        {
            return new UriTemplateResolver(this);
        }

        public string Resolve(IDictionary<string, object> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            var builder = new StringBuilder(Template.Length*2);

            try
            {
                foreach (var component in _components)
                {
                    component.Resolve(builder, variables);
                }
            }
            catch (UriTemplateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UriTemplateException("Error at resolve URI template.", exception);
            }

            return builder.ToString();
        }

        public Uri ResolveUri(IDictionary<string, object> variables)
        {
            return new Uri(Resolve(variables));
        }

        public UriTemplate ResolveTemplate(IDictionary<string, object> variables)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            if (variables.Count == 0)
            {
                return this;
            }

            var partialComponents = new List<IUriComponent>();

            try
            {
                foreach (var component in _components)
                {
                    partialComponents.AddRange(component.ResolveTemplate(variables));
                }
            }
            catch (UriTemplateException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UriTemplateException("Error at partial resolve URI template.", exception);
            }

            return new UriTemplate(partialComponents);
        }

        public override string ToString() => Template;

        private static string GetTemplateString(List<IUriComponent> components)
        {
            var builder = new StringBuilder();

            foreach (var component in components)
            {
                builder.Append(component);
            }

            return builder.ToString();
        }

        private static List<VarSpec> GetVariables(List<IUriComponent> components)
        {
            var variables = new List<VarSpec>();

            foreach (var component in components)
            {
                var expression = component as Expression;

                if (expression != null)
                {
                    foreach (var varSpec in expression.VarSpecs)
                    {
                        variables.Add(varSpec);
                    }
                }
            }

            return variables;
        }
    }
}