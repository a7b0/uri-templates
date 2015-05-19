namespace Resta.UriTemplates
{
    using System;

    internal static class CharSpec
    {
        private static readonly CharType[] charTypeMap;
        private static readonly Predicate<char> safe;
        private static readonly Predicate<char> extendedSafe;
        private static readonly Predicate<char> varChar;

        static CharSpec()
        {
            charTypeMap = new CharType[128];

            for (var i = (byte)'0'; i <= (byte)'9'; i++)
            {
                charTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            for (var i = (byte)'a'; i <= (byte)'z'; i++)
            {
                charTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            for (var i = (byte)'A'; i <= (byte)'Z'; i++)
            {
                charTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            charTypeMap['%'] = CharType.VarChar;
            charTypeMap['~'] = CharType.Unreserved;
            charTypeMap['-'] = CharType.Unreserved;
            charTypeMap['.'] = CharType.Unreserved | CharType.VarChar;
            charTypeMap['_'] = CharType.Unreserved | CharType.VarChar;
            charTypeMap[':'] = CharType.Reserved;
            charTypeMap['/'] = CharType.Reserved;
            charTypeMap['?'] = CharType.Reserved;
            charTypeMap['#'] = CharType.Reserved;
            charTypeMap['['] = CharType.Reserved;
            charTypeMap[']'] = CharType.Reserved;
            charTypeMap['@'] = CharType.Reserved;
            charTypeMap['!'] = CharType.Reserved;
            charTypeMap['$'] = CharType.Reserved;
            charTypeMap['&'] = CharType.Reserved;
            charTypeMap['\''] = CharType.Reserved;
            charTypeMap['('] = CharType.Reserved;
            charTypeMap[')'] = CharType.Reserved;
            charTypeMap['*'] = CharType.Reserved;
            charTypeMap['+'] = CharType.Reserved;
            charTypeMap[','] = CharType.Reserved;
            charTypeMap[';'] = CharType.Reserved;
            charTypeMap['='] = CharType.Reserved;

            safe = IsSafe;
            extendedSafe = IsExtendedSafe;
            varChar = IsVarChar;
        }

        [Flags]
        private enum CharType : byte
        {
            None = 0,
            Unreserved = 1,
            Reserved = 2,
            VarChar = 4
        }

        internal static Predicate<char> Safe
        {
            get { return safe; }
        }

        internal static Predicate<char> ExtendedSafe
        {
            get { return extendedSafe; }
        }

        internal static Predicate<char> VarChar
        {
            get { return varChar; }
        }

        public static bool IsSafe(char ch)
        {
            return HasType(ch, CharType.Unreserved);
        }

        public static bool IsExtendedSafe(char ch)
        {
            return HasType(ch, CharType.Reserved | CharType.Unreserved);
        }

        public static bool IsVarChar(char ch)
        {
            return HasType(ch, CharType.VarChar);
        }

        private static bool HasType(char ch, CharType charSpecType)
        {
            return ch < 128 && (charTypeMap[ch] & charSpecType) != 0;
        }
    }
}