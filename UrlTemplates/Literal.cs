namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class Literal : IUriTemplateComponent
    {
        private readonly string value;

        public Literal(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.value = value;
        }

        public string Value
        {
            get { return value; }
        }

        public void Resolve(StringBuilder builder, IDictionary<string, object> variables)
        {
            builder.Append(value);
        }

        public override string ToString()
        {
            return value;
        }
    }
}