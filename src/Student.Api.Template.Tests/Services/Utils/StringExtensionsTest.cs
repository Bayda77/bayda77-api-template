using FluentAssertions;
using Student.Api.Template.Services.Utils;
using Xunit;

namespace Tests.Services.Utils
{
    public class StringExtensionsTest
    {
        [Theory]
        [InlineData("this is it", "this+is+it")]
        [InlineData("GLEN'S TRUCKERS!!!", "GLEN%27S+TRUCKERS!!!")]
        public void ShouldEncodeString(string toEncode, string encodedTo)
        {
            toEncode.UrlEncoded().Should().Be(encodedTo);
        }

        [Fact]
        public void ShouldAddParameterKeyWithNoValue()
        {
            const string expected = "?name=";
            "".AddAllowableEmptyValueToQueryString("name", "").Should().Be(expected);
            "".AddAllowableEmptyValueToQueryString("name", null).Should().Be(expected);
        }
    }
}