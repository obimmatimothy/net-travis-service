using System;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using SandyBridge.Infrastructure;
using TravisService.Adapters.Sns.Extensions;
using TravisService.Adapters.Sns.Infrastructure;
using TravisService.Adapters.Sns.Models;
using TravisService.Exceptions;
using TravisService.Infrastructure;
using TravisService.Models;
using TravisService.Ports;


namespace TravisService.Adapters.Sns.Ports
{
    internal sealed class SnsNotificationService : INotificationService
    {
        private readonly IAmazonSimpleNotificationService _simpleNotificationService;
        private readonly SnsSettings _snsSettings;
        private readonly MonitoringEvents _monitoringEvents;

        public SnsNotificationService(IAmazonSimpleNotificationService simpleNotificationService, SnsSettings snsSettings, MonitoringEvents monitoringEvents)
        {
            _simpleNotificationService = simpleNotificationService ?? throw new ArgumentNullException(nameof(simpleNotificationService));
            _snsSettings = snsSettings ?? throw new ArgumentNullException(nameof(snsSettings));
            _monitoringEvents = monitoringEvents ?? throw new ArgumentNullException(nameof(monitoringEvents));
        }

        public async Task PublishAlertAsync(AlertEvent alertEvent)
        {
            var eventDto = AlertEventDto.FromDomain(alertEvent);
            var request = new PublishRequest
            {
                TopicArn = _snsSettings.TopicArn,
                Message = JsonSerializer.Serialize(eventDto),
            };

            try
            {
                await Run.TimedAsync(
                    () => _simpleNotificationService.PublishAsync(request),
                    (success, duration) => _monitoringEvents.PublishAlertEvent(success, duration));
            }
            catch (Exception exception)
            {
                _monitoringEvents.FailedToPublishAlertEvent(alertEvent, exception);

                throw new FailedToPublishAlertException(exception);
            }
        }
    }
}
