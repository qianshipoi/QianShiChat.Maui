using Microsoft.EntityFrameworkCore;

using QianShiChatClient.Core.Common;
using QianShiChatClient.Data;
using QianShiChatClient.Data.Repository;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddChatDbContext(this IServiceCollection services, string filePath)
    {
        services.AddDbContext<ChatDbContext>(options => 
            options.UseSqlite($"Filename={filePath}", 
            builder => builder.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName)));

        services.AddScoped<IChatContext>(provider => provider.GetRequiredService<ChatDbContext>());

        services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddTransient(typeof(IReadRepository<>), typeof(BaseRepository<>));
        services.AddTransient<IUserInfoRepository, UserInfoRepository>();

        return services;
    }
}
