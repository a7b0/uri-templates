using System;
using System.Collections.Generic;

namespace Resta.UriTemplates
{
    /// <summary>
    ///     Used to effective programmatically construct URI template.
    /// </summary>
    public class UriTemplateBuilder
    {
        private readonly List<IUriComponent> _components;

        public UriTemplateBuilder()
        {
            _components = new List<IUriComponent>();
        }

        public UriTemplateBuilder(string template)
            : this(new UriTemplate(template))
        {
        }

        public UriTemplateBuilder(UriTemplate template)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            _components = new List<IUriComponent>(template.Components);
        }

        public UriTemplateBuilder(UriTemplateBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _components = new List<IUriComponent>(builder._components);
        }

        public UriTemplateBuilder Literal(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            _components.Add(new Literal(value));
            return this;
        }

        public UriTemplateBuilder Simple(params VarSpec[] vars) => Expression(Operator.Default, vars);

        public UriTemplateBuilder Reserved(params VarSpec[] vars) => Expression(Operator.Reserved, vars);

        public UriTemplateBuilder Fragment(params VarSpec[] vars) => Expression(Operator.Fragment, vars);

        public UriTemplateBuilder Label(params VarSpec[] vars) => Expression(Operator.Label, vars);

        public UriTemplateBuilder Matrix(params VarSpec[] vars) => Expression(Operator.Matrix, vars);

        public UriTemplateBuilder Path(params VarSpec[] vars) => Expression(Operator.Path, vars);

        public UriTemplateBuilder Query(params VarSpec[] vars) => Expression(Operator.Query, vars);

        public UriTemplateBuilder QueryContinuation(params VarSpec[] vars) => Expression(Operator.Continuation, vars);

        public UriTemplateBuilder Expression(char exprOperator, params VarSpec[] vars)
        {
            var op = Operator.Parse(exprOperator);
            return Expression(op, vars);
        }

        public UriTemplateBuilder Clear()
        {
            _components.Clear();
            return this;
        }

        public UriTemplate Build() => new UriTemplate(_components);

        private UriTemplateBuilder Expression(Operator op, params VarSpec[] vars)
        {
            if (vars == null)
            {
                throw new ArgumentNullException(nameof(vars));
            }

            var varsList = new List<VarSpec>(vars.Length);

            foreach (var varSpec in vars)
            {
                if (varSpec == null)
                {
                    throw new ArgumentException("Variable cannot be null.", nameof(vars));
                }

                varsList.Add(varSpec);
            }

            var expression = new Expression(op, varsList);
            _components.Add(expression);
            return this;
        }
    }
}