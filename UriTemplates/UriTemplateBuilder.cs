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

        public UriTemplateBuilder(UriTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.components = new List<IUriComponent>(template.Components);
        }

        public UriTemplateBuilder(UriTemplateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }

            this.components = new List<IUriComponent>(builder.components);
        }

        [Obsolete]
        public UriTemplateBuilder Append(string value)
        {
            return Literal(value);
        }

        [Obsolete]
        public UriTemplateBuilder Append(params VarSpec[] varSpecs)
        {
            return Expression(Operator.Default, varSpecs);
        }

        [Obsolete]
        public UriTemplateBuilder Append(char exprOperator, params VarSpec[] varSpecs)
        {
            var op = Operator.Parse(exprOperator);
            return Expression(op, varSpecs);
        }

        public UriTemplateBuilder Literal(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            components.Add(new Literal(value));
            return this;
        }

        public UriTemplateBuilder Simple(params VarSpec[] vars)
        {
            return Expression(Operator.Default, vars);
        }

        public UriTemplateBuilder Reserved(params VarSpec[] vars)
        {
            return Expression(Operator.Reserved, vars);
        }

        public UriTemplateBuilder Fragment(params VarSpec[] vars)
        {
            return Expression(Operator.Fragment, vars);
        }

        public UriTemplateBuilder Label(params VarSpec[] vars)
        {
            return Expression(Operator.Label, vars);
        }

        public UriTemplateBuilder Matrix(params VarSpec[] vars)
        {
            return Expression(Operator.Matrix, vars);
        }

        public UriTemplateBuilder Path(params VarSpec[] vars)
        {
            return Expression(Operator.Path, vars);
        }

        public UriTemplateBuilder Query(params VarSpec[] vars)
        {
            return Expression(Operator.Query, vars);
        }

        public UriTemplateBuilder QueryContinuation(params VarSpec[] vars)
        {
            return Expression(Operator.Continuation, vars);
        }

        public UriTemplateBuilder Expression(char exprOperator, params VarSpec[] vars)
        {
            var op = Operator.Parse(exprOperator);
            return Expression(op, vars);
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

        private UriTemplateBuilder Expression(Operator op, params VarSpec[] vars)
        {
            if (vars == null)
            {
                throw new ArgumentNullException("vars");
            }

            var varsList = new List<VarSpec>(vars.Length);

            foreach (var varSpec in vars)
            {
                if (varSpec == null)
                {
                    throw new ArgumentException("Variable cannot be null.", "vars");
                }

                varsList.Add(varSpec);
            }

            var expression = new Expression(op, varsList);
            components.Add(expression);
            return this;
        }
    }
}