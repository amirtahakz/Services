using Microsoft.Extensions.DependencyInjection;

namespace Common.Application.KafkaService
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
