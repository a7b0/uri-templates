namespace Resta.UriTemplates
{
    using System;

    internal class Operator
    {
        public static readonly Operator Default = new Operator('\0', null, ",", null, false, false);
        public static readonly Operator Reserved = new Operator('+', null, ",", null, false, true);
        public static readonly Operator Label = new Operator('.', ".", ".", null, false, false);
        public static readonly Operator Path = new Operator('/', "/", "/", null, false, false);
        public static readonly Operator Matrix = new Operator(';', ";", ";", null, true, false);
        public static readonly Operator Query = new Operator('?', "?", "&", "=", true, false);
        public static readonly Operator Continuation = new Operator('&', "&", "&", "=", true, false);
        public static readonly Operator Fragment = new Operator('#', "#", ",", null, false, true);

        private Operator(char code, string prefix, string separator, string empty, bool named, bool allowReserved)
        {
            Code = code;
            Prefix = prefix ?? string.Empty;
            Separator = separator ?? string.Empty;
            Empty = empty ?? string.Empty;
            Named = named;
            AllowReserved = allowReserved;
        }

        public char Code { get; private set; }

        public string Prefix { get; private set; }

        public string Separator { get; private set; }

        public string Empty { get; private set; }

        public bool Named { get; private set; }

        public bool AllowReserved { get; private set; }

        public static Operator Parse(char code)
        {
            Operator op;

            if (!TryParse(code, out op))
            {
                throw new UriTemplateException(string.Format("Expression operator \"{0}\" is unknown", op));
            }

            return op;
        }

        public static bool TryParse(char code, out Operator op)
        {
            op = null;

            switch (code)
            {
                case '\0':
                    op = Default;
                    break;

                case '+':
                    op = Reserved;
                    break;

                case '#':
                    op = Fragment;
                    break;

                case '.':
                    op = Label;
                    break;

                case '/':
                    op = Path;
                    break;

                case ';':
                    op = Matrix;
                    break;

                case '?':
                    op = Query;
                    break;

                case '&':
                    op = Continuation;
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}