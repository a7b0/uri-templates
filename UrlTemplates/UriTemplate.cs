namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// URI template processor (RFC6570)
    /// </summary>
    public sealed class UriTemplate
    {
        private readonly string template;
        private readonly List<IUriTemplateComponent> components;
        private List<VarSpec> variables;

        public UriTemplate(string template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.template = template;
            this.components = UriTemplateParser.Parse(template);
        }

        internal UriTemplate(IEnumerable<IUriTemplateComponent> components)
        {
            if (components == null)
            {
                throw new ArgumentNullException("components");
            }

            this.components = new List<IUriTemplateComponent>(components);
            this.template = GetTemplateString(this.components);
        }

        public string Template
        {
            get { return template; }
        }

        public IEnumerable<VarSpec> Variables
        {
            get
            {
                if (variables == null)
                {
                    variables = GetVariables(components);
                }

                return variables;
            }
        }

        internal IEnumerable<IUriTemplateComponent> Components
        {
            get { return components; }
        }

        public UriTemplateResolver GetResolver()
        {
            return new UriTemplateResolver(this);
        }

        public string Resolve(IDictionary<string, object> variables)
        {
            var builder = new StringBuilder();

            try
            {
                foreach (var component in components)
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
                throw new UriTemplateException("Error at build uri template", exception);
            }

            return builder.ToString();
        }

        public Uri ResolveUri(IDictionary<string, object> variables)
        {
            return new Uri(Resolve(variables));
        }

        public override string ToString()
        {
            return template;
        }

        private static string GetTemplateString(List<IUriTemplateComponent> components)
        {
            var builder = new StringBuilder();

            foreach (var component in components)
            {
                builder.Append(component);
            }

            return builder.ToString();
        }

        private static List<VarSpec> GetVariables(List<IUriTemplateComponent> components)
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