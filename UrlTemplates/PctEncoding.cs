namespace Resta.UriTemplates
{
    using System;
    using System.Text;

    internal class PctEncoding
    {
        private static readonly string HexAlphabit = "0123456789ABCDEF";

        public static string Escape(string value, bool allowReserved)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var i = 0;
            var nonAsciiIndex = -1;
            var builder = new StringBuilder(value.Length * 2);

            while (i < value.Length)
            {
                var symbol = value[i];

                if (symbol > 127)
                {
                    if (nonAsciiIndex == -1)
                    {
                        nonAsciiIndex = i;
                    }
                }
                else 
                {
                    if (nonAsciiIndex != -1)
                    {
                        AppendNonAsciiString(builder, value, nonAsciiIndex, i - nonAsciiIndex);
                        nonAsciiIndex = -1;
                    }

                    if (IsUnreservedChar(symbol) || (allowReserved && IsReservedChar(symbol)))
                    {
                        builder.Append(symbol);
                    }
                    else
                    {
                        AppendHexString(builder, (byte)symbol);
                    }
                }

                i++;
            }

            if (nonAsciiIndex != -1)
            {
                AppendNonAsciiString(builder, value, nonAsciiIndex, i - nonAsciiIndex);
            }

            return builder.ToString();
        }

        private static void AppendNonAsciiString(StringBuilder builder, string value, int charIndex, int charCount)
        {
            var byteCount = Encoding.UTF8.GetMaxByteCount(charCount);

            var buffer = new byte[byteCount];
            byteCount = Encoding.UTF8.GetBytes(value, charIndex, charCount, buffer, 0);

            if (byteCount == 0)
            {
                throw new UriTemplateException(string.Format("Error at escape value \"{0}\"", value));
            }

            for (var j = 0; j < byteCount; j++)
            {
                AppendHexString(builder, buffer[j]);
            }
        }

        private static void AppendHexString(StringBuilder builder, byte value)
        {
            builder.Append('%');
            builder.Append(HexAlphabit[value >> 4]);
            builder.Append(HexAlphabit[value & 15]);
        }

        private static bool IsUnreservedChar(char value)
        {
            if ((value >= 'a' && value <= 'z') ||
                (value >= 'A' && value <= 'Z') ||
                (value >= '0' && value <= '9'))
            {
                return true;
            }

            switch (value)
            {
                case '-':
                case '.':
                case '_':
                case '~':
                    return true;
            }

            return false;
        }

        private static bool IsReservedChar(char value)
        {
            switch (value)
            {
                case ':':
                case '/':
                case '?':
                case '#':
                case '[':
                case ']':
                case '@':
                case '!':
                case '$':
                case '&':
                case '\'':
                case '(':
                case ')':
                case '*':
                case '+':
                case ',':
                case ';':
                case '=':
                    return true;
            }

            return false;
        }
    }
}