using Microsoft.Extensions.DependencyInjection;

namespace Services.Common.Application.KafkaService
{
    public static class KafkaBootstrapper
    {
        public static IServiceCollection AddKafkaService(this IServiceCollection services)
        {
            services.AddSingleton<IKafkaService, KafkaService>();
            return services;
        }
    }
}
