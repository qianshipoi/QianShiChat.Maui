using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Application.Helpers;

internal class ApiUnauthorizationHttpMessageHandler : DelegatingHandler
{
    private readonly INavigationService _navigationService;

    public ApiUnauthorizationHttpMessageHandler(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
        }
        return response;
    }
}
