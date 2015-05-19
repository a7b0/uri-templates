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
            : this(name, false, 0, false)
        {
        }

        public VarSpec(string name, bool exploded)
            : this(name, exploded, 0, false)
        {
        }

        public VarSpec(string name, int maxLength)
            : this(name, false, maxLength, false)
        {
            if (maxLength < 0)
            {
                throw new ArgumentOutOfRangeException("maxLength", "maxLength must be greater then 0");
            }
        }

        internal VarSpec(string name, bool exploded, int maxLength, bool safe)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (name.Length == 0)
            {
                throw new ArgumentException("Variable name cannot be empty.", "name");
            }

            if (!safe && !IsWellFormedName(name))
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

        public static string Escape(string name)
        {
            return PctEncoding.Escape(name, CharSpec.VarChar);
        }

        public static string Unescape(string name)
        {
            return PctEncoding.Unescape(name, CharSpec.VarChar);
        }

        private static bool IsWellFormedName(string name)
        {
            for (var i = 0; i < name.Length; i++)
            {
                if (!CharSpec.IsVarChar(name[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}