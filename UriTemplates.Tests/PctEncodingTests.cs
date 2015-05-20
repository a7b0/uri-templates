namespace Resta.UriTemplates.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class PctEncodingTests
    {
        [Test]
        public void EscapeAsciiStringTest()
        {
            var value = "hello";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.AreSame(value, actual);
        }

        [Test]
        public void EscapeAsciiStringWithReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.AreEqual("hello,%20world!", actual);
        }

        [Test]
        public void EscapeAsciiStringWithoutReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.AreEqual("hello%2C%20world%21", actual);
        }

        [Test]
        public void EscapeNonAsciiStringTest()
        {
            var value = "мир";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.AreEqual("%D0%BC%D0%B8%D1%80", actual);
        }

        [Test]
        public void EscapeLastNonAsiiPartOfStringTest()
        {
            var value = "hello, мир!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.AreEqual("hello,%20%D0%BC%D0%B8%D1%80!", actual);
        }

        [Test]
        public void EscapeFirstNonAsciiPartOfStringTest()
        {
            var value = "привет, world!";
            var actual = PctEncoding.Escape(value, CharSpec.ExtendedSafe);
            Assert.AreEqual("%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82,%20world!", actual);
        }

        [Test]
        public void EscapeStringTest()
        {
            var value = "ha+ха+ho+хо";
            var actual = PctEncoding.Escape(value, CharSpec.Safe);
            Assert.AreEqual("ha%2B%D1%85%D0%B0%2Bho%2B%D1%85%D0%BE", actual);
        }

        [Test]
        public void UnescapeAsciiStringWithReservedCharsTest()
        {
            var value = "hello,%20world!";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("hello, world!", actual);
        }

        [Test]
        public void UnescapeAsciiStringWithoutReservedCharsTest()
        {
            var value = "hello%2C%20world%21";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("hello, world!", actual);
        }

        [Test]
        public void UnescapeNonAsciiStringTest()
        {
            var value = "%D0%BC%D0%B8%D1%80";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("мир", actual);
        }

        [Test]
        public void UnescapeLastNonAsiiPartOfStringTest()
        {
            var value = "hello,%20%D0%BC%D0%B8%D1%80!";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("hello, мир!", actual);
        }

        [Test]
        public void UnescapeFirstNonAsciiPartOfStringTest()
        {
            var value = "%D0%BF%D1%80%D0%B8%D0%B2%D0%B5%D1%82,%20world!";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("привет, world!", actual);
        }

        [Test]
        public void UnescapeStringTest()
        {
            var value = "ha%2B%D1%85%D0%B0%2Bho%2B%D1%85%D0%BE";
            var actual = PctEncoding.Unescape(value);
            Assert.AreEqual("ha+ха+ho+хо", actual);
        }

        [Test]
        public void StrongUnescapeBadStringTest()
        {
            var value = "ha%2i%%2B";
            Assert.Throws<UriTemplateException>(() => PctEncoding.Unescape(value, CharSpec.Safe));
        }

        [Test]
        public void UnescapeBadStringTest()
        {
            var value = "ha%2i%%2B";
            var actual = PctEncoding.Unescape(value);

            Assert.AreEqual("ha%2i%+", actual);
        }

        [Test]
        public void UnescapeAsciiStringTest()
        {
            var value = "hello";
            var actual = PctEncoding.Unescape(value);
            Assert.AreSame(value, actual);
        }
    }
}