using FluentAssertions;
using Student.Api.Template.Services.Utils;
using Xunit;

namespace Tests.Services.Utils
{
    public class AddToQueryStringTests
    {
        [Fact]
        public void AddQueryStringParam()
        {
            "".AddToQueryString("key", "value").Should().Be("?key=value");
            "?key=value".AddToQueryString("second", "anotherValue")
                        .Should().Be("?key=value&second=anotherValue");
            "".AddToQueryString("key", "value1 value2").Should().Be("?key=value1+value2");
        }

        [Fact]
        public void AddQueryStringMissingKeyOrValue()
        {
            "".AddToQueryString("", "value").Should().BeEmpty();
            "".AddToQueryString(null, "value").Should().BeEmpty();
            "".AddToQueryString("key", "").Should().BeEmpty();
            "".AddToQueryString("key", null).Should().BeEmpty();
        }
        
    }
}