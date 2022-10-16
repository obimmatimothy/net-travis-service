using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TravisService.Infrastructure;
using TravisService.UseCases;

namespace TravisService
{
    public static class DomainAdapter
    {
        public static void AddDomainLogic(this IServiceCollection serviceCollection, MonitoringEvents monitoringEvents)
        {
            serviceCollection.TryAddSingleton(monitoringEvents);
            serviceCollection.AddScoped<IProcessHandheldEventsUseCase, ProcessHandheldEventsUseCase>();
        }
    }
}
