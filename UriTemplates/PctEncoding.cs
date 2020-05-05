using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Resta.UriTemplates.Tests")]
namespace Resta.UriTemplates
{
    using System;
    using System.Text;

    internal static class PctEncoding
    {
        private static readonly string HexAlphabit = "0123456789ABCDEF";

        public static string Escape(string value, Predicate<char> predicate)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var index = 0;
            var lastIndex = 0;
            var encode = false;
            StringBuilder builder = null;

            while (index < value.Length)
            {
                var ch = value[index];

                if (ch < 128)
                {
                    if (encode)
                    {
                        EscapeBytes(builder, value, lastIndex, index - lastIndex);
                        lastIndex = index;
                        encode = false;
                    }

                    if (!predicate(ch))
                    {
                        if (builder == null)
                        {
                            builder = new StringBuilder(value.Length * 2);
                        }

                        builder.Append(value, lastIndex, index - lastIndex);
                        EscapeByte(builder, (byte)ch);
                        lastIndex = index + 1;
                    }
                }
                else if (!encode)
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length * 2);
                    }

                    builder.Append(value, lastIndex, index - lastIndex);

                    encode = true;
                    lastIndex = index;
                }

                index++;
            }

            if (encode)
            {
                if (builder == null)
                {
                    builder = new StringBuilder(value.Length * 2);
                }

                EscapeBytes(builder, value, lastIndex, value.Length - lastIndex);
                lastIndex = value.Length;
            }

            if (builder != null)
            {
                builder.Append(value, lastIndex, value.Length - lastIndex);
                value = builder.ToString();
            }

            return value;
        }

        public static string Unescape(string value)
        {
            return Unescape(value, null);
        }

        public static string Unescape(string value, Predicate<char> predicate)
        {
            var index = 0;
            var lastIndex = 0;
            byte[] buffer = null;
            var bufferIndex = 0;
            StringBuilder builder = null;

            while (index != value.Length)
            {
                var ch = value[index];

                if (ch == '%' && index < value.Length - 2)
                {
                    var digit1 = GetHexDigit(value[index + 1]);
                    var digit2 = GetHexDigit(value[index + 2]);

                    if (digit1 != -1 && digit2 != -1)
                    {
                        if (buffer == null)
                        {
                            var bufferSize = (value.Length - index) / 3;
                            buffer = new byte[bufferSize];
                        }

                        if (bufferIndex == 0)
                        {
                            if (builder == null)
                            {
                                builder = new StringBuilder(value.Length);
                            }

                            builder.Append(value, lastIndex, index - lastIndex);
                        }

                        buffer[bufferIndex++] = (byte)((digit1 << 4) + digit2);
                        index += 3;
                        continue;
                    }
                }

                if (predicate != null && !predicate(ch))
                {
                    throw new UriTemplateException(string.Format("Invalid pct-encoding char \"{0}\" in \"{1}\".", ch, value));
                }

                if (bufferIndex != 0)
                {
                    UnescapeBytes(builder, buffer, bufferIndex);
                    bufferIndex = 0;
                    lastIndex = index;
                }

                index++;
            }

            if (bufferIndex != 0)
            {
                if (builder == null)
                {
                    builder = new StringBuilder(value.Length);
                }

                UnescapeBytes(builder, buffer, bufferIndex);
                lastIndex = value.Length;
            }

            if (builder != null)
            {
                builder.Append(value, lastIndex, value.Length - lastIndex);
                value = builder.ToString();
            }

            return value;
        }

        private static void EscapeByte(StringBuilder builder, byte value)
        {
            builder.Append('%');
            builder.Append(HexAlphabit[value >> 4]);
            builder.Append(HexAlphabit[value & 15]);
        }

        private static void EscapeBytes(StringBuilder builder, string value, int offset, int count)
        {
            var encoding = Encoding.UTF8;
            var bufferSize = encoding.GetMaxByteCount(count);
            var buffer = new byte[bufferSize];

            var length = encoding.GetBytes(value, offset, count, buffer, 0);

            for (var i = 0; i < length; i++)
            {
                EscapeByte(builder, buffer[i]);
            }
        }

        private static void UnescapeBytes(StringBuilder builder, byte[] buffer, int count)
        {
            if (count == 1 && buffer[0] < 127)
            {
                builder.Append((char)buffer[0]);
            }
            else
            {
                var value = Encoding.UTF8.GetString(buffer, 0, count);
                builder.Append(value);
            }
        }

        private static int GetHexDigit(char hex)
        {
            var value = hex - (hex < 58 ? 48 : (hex < 97 ? 55 : 87));

            if (value < 0 || value > 15)
            {
                value = -1;
            }

            return value;
        }
    }
}