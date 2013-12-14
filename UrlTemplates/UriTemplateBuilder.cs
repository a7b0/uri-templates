namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;

    public class UriTemplateBuilder
    {
        private readonly List<IUriTemplateComponent> components;

        public UriTemplateBuilder()
        {
            this.components = new List<IUriTemplateComponent>();
        }

        public UriTemplateBuilder(string template)
            : this(new UriTemplate(template))
        {
        }

        public UriTemplateBuilder(UriTemplate uriTemplate)
        {
            if (uriTemplate == null)
            {
                throw new ArgumentNullException("uriTemplate");
            }

            this.components = new List<IUriTemplateComponent>(uriTemplate.Components);
        }

        public UriTemplateBuilder(UriTemplateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            this.components = new List<IUriTemplateComponent>(builder.components);
        }

        public UriTemplateBuilder Append(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            components.Add(new Literal(value));
            return this;
        }

        public UriTemplateBuilder Append(params VarSpec[] varSpecs)
        {
            var expression = new Expression(Operator.Default, new List<VarSpec>(varSpecs));
            components.Add(expression);
            return this;
        }

        public UriTemplateBuilder Append(char exprOperator, params VarSpec[] varSpecs)
        {
            Operator op;

            if (!Operator.TryParse(exprOperator, out op))
            {
                throw new UriTemplateException(string.Format("Expression operator \"{0}\" is unknown", op));
            }

            var expression = new Expression(op, new List<VarSpec>(varSpecs));
            components.Add(expression);
            return this;
        }

        public void Clear()
        {
            components.Clear();
        }

        public UriTemplate Build()
        {
            return new UriTemplate(components);
        }
    }
}