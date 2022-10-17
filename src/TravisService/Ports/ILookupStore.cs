using System.Threading.Tasks;
using TravisService.Exceptions;

namespace TravisService.Ports
{
    public interface ILookupStore
    {
        /// <summary>
        /// Returns the handheldId paired with the provided vehicleId
        /// </summary>
        /// <exception cref="UnableToRetrieveHandheldException"></exception>
        Task<string> GetHandheldIdAsync(string vehicleId);
    }
}
