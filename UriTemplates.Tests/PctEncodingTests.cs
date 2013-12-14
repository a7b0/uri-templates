namespace UriTemplates.Tests
{
    using NUnit.Framework;
    using Resta.UriTemplates;

    [TestFixture]
    public class PctEncodingTests
    {
        [Test]
        public void EscapeAsciiStringWithReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, true);
            Assert.AreEqual("hello,%20world!", actual);
        }

        [Test]
        public void EscapeAsciiStringWithoutReservedCharsTest()
        {
            var value = "hello, world!";
            var actual = PctEncoding.Escape(value, false);
            Assert.AreEqual("hello%2C%20world%21", actual);
        }

        [Test]
        public void EscapeStringTest()
        {
            var value = "hello, мир!";
            var actual = PctEncoding.Escape(value, true);
            Assert.AreEqual("hello,%20%D0%BC%D0%B8%D1%80!", actual);
        }

        [Test]
        public void EscapeNonAsciiStringTest()
        {
            var value = "мир";
            var actual = PctEncoding.Escape(value, true);
            Assert.AreEqual("%D0%BC%D0%B8%D1%80", actual);
        }
    }
}