namespace Resta.UriTemplates
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class Literal : IUriComponent
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

        public IEnumerable<IUriComponent> ResolveTemplate(IDictionary<string, object> variables)
        {
            return new IUriComponent[] { this };
        }

        public override string ToString()
        {
            return value;
        }
    }
}