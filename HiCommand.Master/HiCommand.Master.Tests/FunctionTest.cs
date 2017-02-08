using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using Newtonsoft.Json;
using Xunit;

namespace HiCommand.Master.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestBaseHi()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                Body = "token=khr0zazcrfkk1zmo40qbwerg&team_id=t3lll2dew&team_domain=tiltshift-software&channel_id=c3lll2ht4&channel_name=random&user_id=u3lll2dga&user_name=kmerecido&command=%2fhi&text=&response_url=https%3a%2f%2fhooks.slack.com%2fcommands%2ft3lll2dew%2f136180532529%2fzlpgyjfounk7wsllmewrvzpm"
            };
            var response = function.FunctionHandler(request, context);

            Assert.Equal("hello there, kmerecido!", response.Result.Body);
        }

        [Fact]
        public void TestDebug()
        {

            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var request = new APIGatewayProxyRequest
            {
                Body = "test=debug&command=%2fhi&channel_id=c3lll2ht4&text=debug&response_url=https%3a%2f%2fhooks.slack.com%2fcommands%2ft3lll2dew%2f136180532529%2fzlpgyjfounk7wsllmewrvzpm"
            };
            var response = function.FunctionHandler(request, context);

            Assert.Equal("{\"channel\":\"c3lll2ht4\",\"username\":\"Hi-Command\",\"text\":\"{\\\"test\\\":[\\\"debug\\\"],\\\"command\\\":[\\\"/hi\\\"],\\\"channel_id\\\":[\\\"c3lll2ht4\\\"],\\\"text\\\":[\\\"debug\\\"],\\\"response_url\\\":[\\\"https://hooks.slack.com/commands/t3lll2dew/136180532529/zlpgyjfounk7wsllmewrvzpm\\\"]}\"}", response.Result.Body);
        }

        public class DebugResponseBody
        {
            [JsonProperty("test")]
            public string Test { get; set; }
        }
    }
}
