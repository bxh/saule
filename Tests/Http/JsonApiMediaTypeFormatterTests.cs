using System.Linq;
using Saule;
using Saule.Http;
using Xunit;

namespace Tests.Http
{
    public class JsonApiMediaTypeFormatterTests
    {
        [Fact(DisplayName = "Formatter must support Json Api media type")]
        public void TestMethod1()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var formatter = new JsonApiMediaTypeFormatter();
#pragma warning restore CS0618 // Type or member is obsolete
            Assert.Equal(1, formatter.SupportedMediaTypes.Count);
            Assert.Equal(Constants.MediaType, formatter.SupportedMediaTypes.First().MediaType);
        }
    }
}