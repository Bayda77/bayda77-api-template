using System.Linq;
using FluentAssertions;
using Student.Api.Template.Services.Net;
using Xerris.DotNet.Core.Extensions;
using Xunit;

namespace Tests.Services.Net
{
    public class WebMethodExceptionTest
    {
        private const string SampleTemporaryPricingApprovalError = @"{
                                  'error': {
                                    'message': [
                                      'Error from MSS: 08000211 - Illegal value'
                                    ]
                                  }
                                }";

        [Fact]
        public void ShouldParseFromExpectedJson()
        {
            var exception = SampleTemporaryPricingApprovalError.FromJson<WebMethodException>();
            exception.Error.Should().NotBeNull();
            exception.Error.Message.Should().NotBeNull();
            exception.Error.Message.Should().HaveCount(1);
            exception.Error.Message.First().Should().Be("Error from MSS: 08000211 - Illegal value");
        }
    }
}