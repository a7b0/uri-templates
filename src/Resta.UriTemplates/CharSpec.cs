using System;

namespace Resta.UriTemplates
{
    internal static class CharSpec
    {
        private static readonly CharType[] CharTypeMap;

        static CharSpec()
        {
            CharTypeMap = new CharType[128];

            for (var i = (byte) '0'; i <= (byte) '9'; i++)
            {
                CharTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            for (var i = (byte) 'a'; i <= (byte) 'z'; i++)
            {
                CharTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            for (var i = (byte) 'A'; i <= (byte) 'Z'; i++)
            {
                CharTypeMap[i] = CharType.Unreserved | CharType.VarChar;
            }

            CharTypeMap['%'] = CharType.VarChar;
            CharTypeMap['~'] = CharType.Unreserved;
            CharTypeMap['-'] = CharType.Unreserved;
            CharTypeMap['.'] = CharType.Unreserved | CharType.VarChar;
            CharTypeMap['_'] = CharType.Unreserved | CharType.VarChar;
            CharTypeMap[':'] = CharType.Reserved;
            CharTypeMap['/'] = CharType.Reserved;
            CharTypeMap['?'] = CharType.Reserved;
            CharTypeMap['#'] = CharType.Reserved;
            CharTypeMap['['] = CharType.Reserved;
            CharTypeMap[']'] = CharType.Reserved;
            CharTypeMap['@'] = CharType.Reserved;
            CharTypeMap['!'] = CharType.Reserved;
            CharTypeMap['$'] = CharType.Reserved;
            CharTypeMap['&'] = CharType.Reserved;
            CharTypeMap['\''] = CharType.Reserved;
            CharTypeMap['('] = CharType.Reserved;
            CharTypeMap[')'] = CharType.Reserved;
            CharTypeMap['*'] = CharType.Reserved;
            CharTypeMap['+'] = CharType.Reserved;
            CharTypeMap[','] = CharType.Reserved;
            CharTypeMap[';'] = CharType.Reserved;
            CharTypeMap['='] = CharType.Reserved;

            Safe = IsSafe;
            ExtendedSafe = IsExtendedSafe;
            VarChar = IsVarChar;
        }

        internal static Predicate<char> Safe { get; }

        internal static Predicate<char> ExtendedSafe { get; }

        internal static Predicate<char> VarChar { get; }

        private static bool IsSafe(char ch)
        {
            return HasType(ch, CharType.Unreserved);
        }

        private static bool IsExtendedSafe(char ch)
        {
            return HasType(ch, CharType.Reserved | CharType.Unreserved);
        }

        public static bool IsVarChar(char ch)
        {
            return HasType(ch, CharType.VarChar);
        }

        private static bool HasType(char ch, CharType charSpecType)
        {
            return ch < 128 && (CharTypeMap[ch] & charSpecType) != 0;
        }

        [Flags]
        private enum CharType : byte
        {
            None = 0,
            Unreserved = 1,
            Reserved = 2,
            VarChar = 4
        }
    }
}