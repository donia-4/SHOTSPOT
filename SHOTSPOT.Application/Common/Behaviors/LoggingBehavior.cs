using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace SHOTSPOT.Application.Common.Behaviors
{
    public class LoggingBehaviour<TRequest>(ILogger<TRequest> logger)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
    {
        private readonly ILogger _logger = logger;

        public async Task Process(TRequest request, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogInformation(
                "Request: {Name} {@Request}", requestName, request);
        }
    }
}
