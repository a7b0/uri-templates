namespace Resta.UriTemplates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
#if !NET20
    using System.Linq;
#endif
    using System.Text;

    internal class Expression : IUriComponent
    {
        private readonly List<VarSpec> _varSpecs;

        public Expression(Operator op, List<VarSpec> varSpecs)
        {
            if (op == null)
            {
                throw new ArgumentNullException(nameof(op));
            }

            if (varSpecs == null)
            {
                throw new ArgumentNullException(nameof(varSpecs));
            }

            Operator = op;
            _varSpecs = varSpecs;
        }

        public Operator Operator { get; }

        public IEnumerable<VarSpec> VarSpecs => _varSpecs;

        public IEnumerable<IUriComponent> ResolveTemplate(IDictionary<string, object> variables)
        {
            var builder = new StringBuilder();
            var varsLeft = new List<VarSpec>();
            var components = new List<IUriComponent>();

            var hasPrefix = false;
            var hasLiterals = false;
            var hasExpressions = false;

            foreach (var varSpec in _varSpecs)
            {
                object value;

                if (!variables.TryGetValue(varSpec.Name, out value))
                {
                    if (builder.Length > 0 && Operator != Operator.Query)
                    {
                        components.Add(new Literal(builder.ToString()));
                        builder.Length = 0;
                        hasLiterals = true;
                    }

                    varsLeft.Add(varSpec);
                }
                else if (value != null)
                {
                    if (Resolve(builder, varSpec, value, hasPrefix))
                    {
                        hasPrefix = true;

                        if (varsLeft.Count > 0 && Operator != Operator.Query)
                        {
                            components.Add(new Expression(Operator, varsLeft));
                            hasExpressions = true;
                            varsLeft = new List<VarSpec>();
                        }
                    }
                }
            }

            if (builder.Length > 0)
            {
                components.Add(new Literal(builder.ToString()));
                hasLiterals = true;
            }

            if (varsLeft.Count > 0)
            {
                var expressionOp = Operator;

                if (hasLiterals && Operator == Operator.Query)
                {
                    expressionOp = Operator.Continuation;
                }

                components.Add(new Expression(expressionOp, varsLeft));
                hasExpressions = true;
            }

            if (hasExpressions && hasLiterals)
            {
                if (Operator == Operator.Default || Operator == Operator.Reserved || Operator == Operator.Fragment)
                {
                    throw new UriTemplateException(string.Format("Partial resolve of expression \"{0}\" is not available.", ToString()));
                }
            }

            return components;
        }

        public void Resolve(StringBuilder builder, IDictionary<string, object> variables)
        {
            var hasPrefix = false;

            foreach (var varSpec in _varSpecs)
            {
                object value;

                if (variables.TryGetValue(varSpec.Name, out value) && value != null)
                {
                    if (Resolve(builder, varSpec, value, hasPrefix))
                    {
                        hasPrefix = true;
                    }
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(21 * _varSpecs.Count + 2);

            builder.Append('{');

            if (Operator.Code != '\0')
            {
                builder.Append(Operator.Code);
            }

            for (var i = 0; i < _varSpecs.Count; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }

                var varSpec = _varSpecs[i];

                builder.Append(varSpec.Name);

                if (varSpec.Exploded)
                {
                    builder.Append("*");
                }
                else if (varSpec.MaxLength > 0)
                {
                    builder.Append(':').Append(varSpec.MaxLength);
                }
            }

            builder.Append('}');

            return builder.ToString();
        }

        private bool Resolve(StringBuilder builder, VarSpec varSpec, object value, bool hasPrefix)
        {
            string stringValue = null;
            IDictionary<string, string> dictionaryValue = null;
            IEnumerable<string> collectionValue = null;

            stringValue = value as string;

            if (stringValue == null)
            {
                collectionValue = value as IEnumerable<string>;

                if (collectionValue == null)
                {
                    dictionaryValue = value as IDictionary<string, string>;

                    if (dictionaryValue == null)
                    {
                        throw new UriTemplateException(string.Format("Invalid value type of variable \"{0}\". Expected: string or IEnumerable<string> or IDictionary<string, string>.", value.GetType()));
                    }
                    else if (dictionaryValue.Count == 0)
                    {
                        return false;
                    }
                }
                else if (collectionValue.Count() == 0)
                {
                    return false;
                }
            }

            builder.Append(hasPrefix ? Operator.Separator : Operator.Prefix);

            if (stringValue != null)
            {
                BuildStringValue(builder, varSpec, stringValue);
            }
            else if (collectionValue != null)
            {
                BuildCollectionValue(builder, varSpec, collectionValue);
            }
            else
            {
                BuildDictionaryValue(builder, varSpec, dictionaryValue);
            }

            return true;
        }

        private void BuildValue(StringBuilder builder, VarSpec varSpec, string value)
        {
            var charSpec = Operator.AllowReserved ? CharSpec.ExtendedSafe : CharSpec.Safe;
            value = PctEncoding.Escape(value, charSpec);
            builder.Append(value);
        }

        private void BuildStringValue(StringBuilder builder, VarSpec varSpec, string value)
        {
            if (Operator.Named)
            {
                builder.Append(varSpec.Name);
                builder.Append(string.IsNullOrEmpty(value) ? Operator.Empty : "=");
            }

            if (varSpec.MaxLength != 0 && varSpec.MaxLength < value.Length)
            {
                value = value.Substring(0, varSpec.MaxLength);
            }

            BuildValue(builder, varSpec, value);
        }

        private void BuildCollectionValue(StringBuilder builder, VarSpec varSpec, IEnumerable<string> values)
        {
            var first = true;

            CheckCompositeVariables(varSpec);

            if (Operator.Named && !varSpec.Exploded)
            {
                builder.Append(varSpec.Name);
            }

            foreach (var value in values)
            {
                if (!first)
                {
                    builder.Append(varSpec.Exploded ? Operator.Separator : ",");
                }

                if (Operator.Named)
                {
                    if (varSpec.Exploded)
                    {
                        builder.Append(varSpec.Name);
                        builder.Append('=');
                    }
                    else if (first)
                    {
                        builder.Append('=');
                    }
                }

                BuildValue(builder, varSpec, value);
                first = false;
            }

            if (first)
            {
                builder.Append(Operator.Empty);
            }
        }

        private void BuildDictionaryValue(StringBuilder builder, VarSpec varSpec, IDictionary<string, string> values)
        {
            var first = true;

            CheckCompositeVariables(varSpec);

            if (Operator.Named && !varSpec.Exploded)
            {
                builder.Append(varSpec.Name);
                builder.Append('=');
            }

            foreach (var item in values)
            {
                if (!first)
                {
                    builder.Append(varSpec.Exploded ? Operator.Separator : ",");
                }

                var key = PctEncoding.Escape(item.Key, CharSpec.ExtendedSafe);

                builder.Append(key);
                builder.Append(varSpec.Exploded ? '=' : ',');
                BuildValue(builder, varSpec, item.Value ?? string.Empty);

                first = false;
            }
        }

        private void CheckCompositeVariables(VarSpec varSpec)
        {
            if (varSpec.MaxLength != 0)
            {
                throw new UriTemplateException("MaxLength modifier are not applicable to composite values");
            }
        }
    }
}