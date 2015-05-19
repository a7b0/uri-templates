namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Used to effective programmatically construct URI template.
    /// </summary>
    public class UriTemplateBuilder
    {
        private readonly List<IUriComponent> components;

        public UriTemplateBuilder()
        {
            this.components = new List<IUriComponent>();
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

            this.components = new List<IUriComponent>(uriTemplate.Components);
        }

        public UriTemplateBuilder(UriTemplateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            this.components = new List<IUriComponent>(builder.components);
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
            var op = Operator.Parse(exprOperator);
            var expression = new Expression(op, new List<VarSpec>(varSpecs));
            components.Add(expression);
            return this;
        }

        public UriTemplateBuilder Clear()
        {
            components.Clear();
            return this;
        }

        public UriTemplate Build()
        {
            return new UriTemplate(components);
        }
    }
}