using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using SandyBridge.Infrastructure;
using TravisService.Functions.HandheldEventProcessor.Extensions;
using TravisService.Functions.HandheldEventProcessor.Models;
using TravisService.Infrastructure;
using TravisService.UseCases;

namespace TravisService.Functions.HandheldEventProcessor
{
    public class Function
    {
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly IServiceProvider _serviceProvider;
        private readonly MonitoringEvents _monitoringEvents;

        public Function()
        {
            _serviceProvider = Bootstrapper.CreateDefaultApplication();
            _monitoringEvents = _serviceProvider.GetRequiredService<MonitoringEvents>();
        }

        public async Task Handler(SQSEvent sqsEvent)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();

                var handheldEvents = sqsEvent.Records.Select(r => r.Body).ToList();

                await Run.TimedAsync(
                    () => ProcessMessageAsync(scope, handheldEvents),
                    (success, duration) => _monitoringEvents.LambdaExecution(success, duration));
            }
            catch (Exception exception)
            {
                _monitoringEvents.LambdaExecutionFailed(exception);

                throw; // Exceptions are being rethrown to signal the lambda runtime in AWS to requeue the message(s)
            }
        }

        private static async Task ProcessMessageAsync(IServiceScope scope, IReadOnlyCollection<string> handheldEvents)
        {
            var handheldEventsDto = handheldEvents.Select(h => JsonSerializer.Deserialize<HandheldEventDto>(h, options));

            var useCase = scope.ServiceProvider.GetRequiredService<IProcessHandheldEventsUseCase>();
            await useCase.ExecuteAsync(handheldEventsDto.Select(he => he.ToDomain()).ToList());
        }
    }
}
