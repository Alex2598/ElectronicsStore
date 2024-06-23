using Xunit;

namespace Store.Tests
{
    public class ComponentTests
    {
        [Fact]
        public void IsIsbn_WithNull_ReturnFalse()
        {
            bool actual = Component.IsUId(null);

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithBlankString_ReturnFalse()
        {
            bool actual = Component.IsUId("   ");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithInvalidIsbn_ReturnFalse()
        {
            bool actual = Component.IsUId("ISBN 123");

            Assert.False(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn10_ReturnTrue()
        {
            bool actual = Component.IsUId("IsBn 123-456-789 0");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithIsbn13_ReturnTrue()
        {
            bool actual = Component.IsUId("IsBn 123-456-789 0123");

            Assert.True(actual);
        }

        [Fact]
        public void IsIsbn_WithTrashStart_ReturnFalse()
        {
            bool actual = Component.IsUId("xxx IsBn 123-456-789 0123 yyy");

            Assert.False(actual);
        }
    }
}
