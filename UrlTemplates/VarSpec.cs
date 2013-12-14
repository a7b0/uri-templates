namespace Resta.UriTemplates
{
    using System;
    using System.Text;

    /// <summary>
    /// Description of URI template variable.
    /// </summary>
    public sealed class VarSpec
    {
        private readonly string name;
        private readonly bool exploded;
        private readonly int maxLength;

        public VarSpec(string name)
            : this(name, false, 0)
        {
        }

        public VarSpec(string name, bool exploded)
            : this(name, exploded, 0)
        {
        }

        public VarSpec(string name, int maxLength)
            : this(name, false, maxLength)
        {
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException("maxLength", "maxLength must be greater then 0");
            }
        }

        internal VarSpec(string name, bool exploded, int maxLength)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!IsValidName(name))
            {
                throw new ArgumentException(string.Format("Invalid variable name \"{0}\"", name), "name");
            }

            this.name = name;
            this.exploded = exploded;
            this.maxLength = maxLength;
        }

        public string Name
        {
            get { return name; }
        }

        public bool Exploded
        {
            get { return exploded; }
        }

        public int MaxLength
        {
            get { return maxLength; }
        }

        internal static bool IsValidName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            foreach (var symbol in name)
            {
                if (!IsValidNameChar(symbol))
                {
                    return false;
                }
            }

            return true;
        }

        internal static bool IsValidNameChar(char symbol)
        {
            return (symbol >= 'A' && symbol <= 'Z') ||
                (symbol >= 'a' && symbol <= 'z') ||
                (symbol >= '0' && symbol <= '9') ||
                symbol == '_' ||
                symbol == '%' ||
                symbol == '.';
        }
    }
}