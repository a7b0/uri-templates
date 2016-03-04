using Xunit;

namespace Resta.UriTemplates.Tests
{
    public class PctEncodingTests
    {
        [Fact]
        public void EscapeAsciiStringTest()
        {
            var value = "hello";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.Equal(value, actual);
        }

        [Fact]
        public void EscapeAsciiStringWithReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.Equal("hello,%20world!", actual);
        }

        [Fact]
        public void EscapeAsciiStringWithoutReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.Equal("hello%2C%20world%21", actual);
        }

        [Fact]
        public void EscapeNonAsciiStringTest()
        {
            var value = "мир";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.Equal("%D0%BC%D0%B8%D1%80", actual);
        }

        [Fact]
        public void EscapeLastNonAsiiPartOfStringTest()
        {
            var value = "hello, мир!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.Equal("hello,%20%D0%BC%D0%B8%D1%80!", actual);
        }

        [Fact]
        public void EscapeFirstNonAsciiPartOfStringTest()
        {
            var value = "привет, world!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.Equal("%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82,%20world!", actual);
        }

        [Fact]
        public void EscapeStringTest()
        {
            var value = "ha+ха+ho+хо";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.Equal("ha%2B%D1%85%D0%B0%2Bho%2B%D1%85%D0%BE", actual);
        }

        [Fact]
        public void UnescapeAsciiStringWithReservedCharsTest()
        {
            var value = "hello,%20world!";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("hello, world!", actual);
        }

        [Fact]
        public void UnescapeAsciiStringWithoutReservedCharsTest()
        {
            var value = "hello%2C%20world%21";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("hello, world!", actual);
        }

        [Fact]
        public void UnescapeNonAsciiStringTest()
        {
            var value = "%D0%BC%D0%B8%D1%80";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("мир", actual);
        }

        [Fact]
        public void UnescapeLastNonAsiiPartOfStringTest()
        {
            var value = "hello,%20%D0%BC%D0%B8%D1%80!";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("hello, мир!", actual);
        }

        [Fact]
        public void UnescapeFirstNonAsciiPartOfStringTest()
        {
            var value = "%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82,%20world!";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("привет, world!", actual);
        }

        [Fact]
        public void UnescapeStringTest()
        {
            var value = "ha%2B%D1%85%D0%B0%2Bho%2B%D1%85%D0%BE";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal("ha+ха+ho+хо", actual);
        }

        [Fact]
        public void StrongUnescapeBadStringTest()
        {
            var value = "ha%2i%%2B";
            Assert.Throws<UriTemplateException>(() => PctEncoding.Unescape(value, CharSpec.Safe));
        }

        [Fact]
        public void UnescapeBadStringTest()
        {
            var value = "ha%2i%%2B";
            var actual = PctEncoding.Unescape(value);

            Assert.Equal("ha%2i%+", actual);
        }

        [Fact]
        public void UnescapeAsciiStringTest()
        {
            var value = "hello";
            var actual = PctEncoding.Unescape(value);
            Assert.Equal(value, actual);
        }
    }
}