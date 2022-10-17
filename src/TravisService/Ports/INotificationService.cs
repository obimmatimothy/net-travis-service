using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Models;

namespace TravisService.Ports
{
    internal interface INotificationService
    {
        /// <summary>
        /// Published a given alert event to a notification service
        /// </summary>
        /// <exception cref="FailedToPublishAlertException"></exception>
        Task PublishAlertAsync(AlertEvent alertEvent);
    }
}
