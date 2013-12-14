namespace Resta.UriTemplates
{
    using System;

    internal class Operator
    {
        private static Operator defaultOp = new Operator('\0', null, ",", null, false, false);
        private static Operator reservedOp = new Operator('+', null, ",", null, false, true);
        private static Operator labelOp = new Operator('.', ".", ".", null, false, false);
        private static Operator pathOp = new Operator('/', "/", "/", null, false, false);
        private static Operator matrixOp = new Operator(';', ";", ";", null, true, false);
        private static Operator queryOp = new Operator('?', "?", "&", "=", true, false);
        private static Operator continuationOp = new Operator('&', "&", "&", "=", true, false);
        private static Operator fragmentOp = new Operator('#', "#", ",", null, false, true);

        public Operator(char code, string prefix, string separator, string empty, bool named, bool allowReserved)
        {
            Code = code;
            Prefix = prefix ?? string.Empty;
            Separator = separator ?? string.Empty;
            Empty = empty ?? string.Empty;
            Named = named;
            AllowReserved = allowReserved;
        }

        public static Operator Default
        {
            get { return defaultOp; }
        }

        public char Code { get; private set; }

        public string Prefix { get; private set; }

        public string Separator { get; private set; }

        public string Empty { get; private set; }

        public bool Named { get; private set; }

        public bool AllowReserved { get; private set; }

        internal static bool TryParse(char code, out Operator op)
        {
            op = null;

            switch (code)
            {
                case '+':
                    op = reservedOp;
                    break;

                case '#':
                    op = fragmentOp;
                    break;

                case '.':
                    op = labelOp;
                    break;

                case '/':
                    op = pathOp;
                    break;

                case ';':
                    op = matrixOp;
                    break;

                case '?':
                    op = queryOp;
                    break;

                case '&':
                    op = continuationOp;
                    break;

                default:
                    return false;
            }

            return true;
        }
    }
}