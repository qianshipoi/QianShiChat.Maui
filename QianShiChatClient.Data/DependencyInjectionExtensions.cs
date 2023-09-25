namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDataStore(this IServiceCollection services, string filePath)
    {
        services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlite($"Filename={filePath}",
            builder => builder.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName)));

        services.AddScoped<IChatContext>(provider => provider.GetRequiredService<ChatDbContext>());

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

        return services;
    }
}
