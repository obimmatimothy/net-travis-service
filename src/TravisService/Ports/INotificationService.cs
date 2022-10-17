using System.Threading.Tasks;
using TravisService.Exceptions;
using TravisService.Models;

namespace TravisService.Ports
{
    public interface INotificationService
    {
        /// <summary>
        /// Published a given alert event to a notification service
        /// </summary>
        /// <exception cref="FailedToPublishAlertException"></exception>
        Task PublishAlertAsync(AlertEvent alertEvent);
    }
}
