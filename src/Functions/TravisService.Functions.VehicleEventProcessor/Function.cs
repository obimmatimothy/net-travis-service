using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.SQSEvents;
using Microsoft.Extensions.DependencyInjection;
using SandyBridge.Infrastructure;
using TravisService.Exceptions;
using TravisService.Functions.VehicleEventProcessor.Extensions;
using TravisService.Functions.VehicleEventProcessor.Models;
using TravisService.Infrastructure;
using TravisService.UseCases;
using static Amazon.Lambda.SQSEvents.SQSEvent;

namespace TravisService.Functions.VehicleEventProcessor;

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

    public async Task<SQSBatchResponse> Handler(SQSEvent sqsEvent)
    {
        List<SQSBatchResponse.BatchItemFailure> batchItemFailures = new();
        using var scope = _serviceProvider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<IProcessVehicleEventUseCase>();

        await Task.WhenAll(
            sqsEvent.Records.Select(
                async record => await ProcessMessageAsync(record)));

        return new SQSBatchResponse(batchItemFailures);

        async Task ProcessMessageAsync(SQSMessage vehicleMessage)
        {
            try
            {
                var vehicleEventDto = JsonSerializer.Deserialize<VehicleEventDto>(vehicleMessage.Body, options);

                await Run.TimedAsync(
                    () => useCase.ExecuteAsync(vehicleEventDto.ToDomain()),
                    (success, duration) => _monitoringEvents.LambdaExecution(success, duration));
            }
            catch (NoHandheldEventsFoundException)
            {
                //Not a lambda failure but maybe we need more time for handheld events to come in so we requeue event
                _monitoringEvents.RequeingEvent(vehicleMessage.Body);

                batchItemFailures.Add(
                    new SQSBatchResponse.BatchItemFailure
                    {
                        ItemIdentifier = vehicleMessage.MessageId
                    });
            }
            catch (Exception exception)
            {
                _monitoringEvents.LambdaExecutionFailed(exception);

                batchItemFailures.Add(
                    new SQSBatchResponse.BatchItemFailure
                    {
                        ItemIdentifier = vehicleMessage.MessageId
                    });
            }
        }
    }
}
