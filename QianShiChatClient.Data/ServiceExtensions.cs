using Microsoft.EntityFrameworkCore;

using QianShiChatClient.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddChatDbContext(this IServiceCollection services, string filePath)
    {
        services.AddDbContext<ChatDbContext>(builder => {
            builder.UseSqlite($"Filename={filePath}");
        });

        return services;
    }
}
