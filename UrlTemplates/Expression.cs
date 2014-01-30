namespace Resta.UriTemplates
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class Expression : IUriTemplateComponent
    {
        private readonly Operator op;
        private readonly List<VarSpec> varSpecs;

        public Expression(Operator op, List<VarSpec> varSpecs)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (varSpecs == null)
            {
                throw new ArgumentNullException("varSpecs");
            }

            this.op = op;
            this.varSpecs = varSpecs;
        }

        public Operator Operator
        {
            get { return op; }
        }

        public IEnumerable<VarSpec> VarSpecs
        {
            get { return varSpecs; }
        }

        public void Resolve(UriTemplateBuilder builder, IDictionary<string, object> variables, bool keepUnresolved)
        {
            var first = true;
            var varsLeft = new List<VarSpec>();
            foreach (var varSpec in varSpecs)
            {
                object value;

                if (!variables.TryGetValue(varSpec.Name, out value) || value == null)
                {
                    varsLeft.Add(varSpec);
                    continue;
                }

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
                            throw new UriTemplateException(string.Format("Invalid type of variable value \"{0}\". Expected: string or IEnumerable<string> or IDictionary<string, string>", value.GetType()));
                        }
                        else if (dictionaryValue.Count == 0)
                        {
                            continue;
                        }
                    }
                    else if (collectionValue.Count() == 0)
                    {
                        continue;
                    }
                }

                var sb = new StringBuilder();
                if (first)
                {
                    sb.Append(op.Prefix);
                    first = false;
                }
                else
                {
                    sb.Append(op.Separator);
                }
                if (stringValue != null)
                {
                    BuildStringValue(sb, varSpec, stringValue);
                }
                else if (collectionValue != null)
                {
                    BuildCollectionValue(sb, varSpec, collectionValue);
                }
                else
                {
                    BuildDictionaryValue(sb, varSpec, dictionaryValue);
                }
                builder.Append(sb.ToString());
            }

            if (keepUnresolved)
            {
                var varsLeftArr = varsLeft.ToArray();
                if (varsLeftArr.Length > 0)
                {
                    if (Operator.Code == '?' && !first)
                    {
                        builder.Append('&', varsLeftArr);
                    }
                    else
                    {
                        builder.Append(Operator.Code, varsLeftArr);
                    }
                }
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder(21 * varSpecs.Count + 2);

            builder.Append('{');

            if (op.Code != '\0')
            {
                builder.Append(op.Code);
            }

            for (var i = 0; i < varSpecs.Count; i++)
            {
                if (i != 0)
                {
                    builder.Append(',');
                }

                var varSpec = varSpecs[i];

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

        private void BuildValue(StringBuilder builder, VarSpec varSpec, string value)
        {
            builder.Append(PctEncoding.Escape(value, op.AllowReserved));
        }

        private void BuildStringValue(StringBuilder builder, VarSpec varSpec, string value)
        {
            if (op.Named)
            {
                builder.Append(varSpec.Name);

                if (string.IsNullOrEmpty(value))
                {
                    builder.Append(op.Empty);
                }
                else
                {
                    builder.Append("=");
                }
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

            if (op.Named && !varSpec.Exploded)
            {
                builder.Append(varSpec.Name);
            }

            foreach (var value in values)
            {
                if (!first)
                {
                    builder.Append(varSpec.Exploded ? op.Separator : ",");
                }

                if (op.Named)
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
                builder.Append(op.Empty);
            }
        }

        private void BuildDictionaryValue(StringBuilder builder, VarSpec varSpec, IDictionary<string, string> values)
        {
            var first = true;

            CheckCompositeVariables(varSpec);

            if (op.Named && !varSpec.Exploded)
            {
                builder.Append(varSpec.Name);
                builder.Append('=');
            }

            foreach (var item in values)
            {
                if (!first)
                {
                    builder.Append(varSpec.Exploded ? op.Separator : ",");
                }

                var key = PctEncoding.Escape(item.Key, true);

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