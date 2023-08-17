namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDataStore(this IServiceCollection services, string filePath)
    {
        services.AddDbContext<ChatDbContext>(options =>
            options.UseSqlite($"Filename={filePath}",
            builder => builder.MigrationsAssembly(typeof(ChatDbContext).Assembly.FullName)));

        services.AddScoped<IChatContext>(provider => provider.GetRequiredService<ChatDbContext>());

        services.AddTransient(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddTransient(typeof(IReadRepository<>), typeof(BaseRepository<>));
        services.AddTransient<IUserInfoRepository, UserInfoRepository>();
        services.AddTransient<ISessionRepository, SessionRepository>();
        services.AddTransient<IChatMessageRepository, ChatMessageRepository>();

        return services;
    }
}
