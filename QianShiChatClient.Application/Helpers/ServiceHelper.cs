using Microsoft.Extensions.DependencyInjection;

namespace QianShiChatClient.Application.Helpers;

public static class ServiceHelper
{
    public static TService? GetService<TService>() => Current.GetService<TService>();

    public static TService GetReqiredService<TService>() where TService : notnull => Current.GetRequiredService<TService>();

    public static IServiceProvider Current { get; set; }
}