﻿namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(CurrentSelectedRoom), nameof(CurrentSelectedRoom))]
public sealed partial class MessageViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly Dictionary<RoomModelBase, View> _viewCache;
    private readonly ILogger<MessageViewModel> _logger;

    [ObservableProperty]
    private View _content;

    public DataCenter DataCenter { get; }

    [ObservableProperty]
    private RoomModelBase _currentSelectedRoom;

    partial void OnCurrentSelectedRoomChanged(RoomModelBase value)
    {
        if (value is null)
        {
            Content = null;
            return;
        }

        var windowManagerService = ServiceHelper.GetService<IWindowManagerService>();
        if (windowManagerService != null)
        {
            if (windowManagerService.ContainsChatRootWindow(value))
            {
                windowManagerService.OpenChatRoomWindow(value);
                CurrentSelectedRoom = null;
                return;
            }
        }

        if (!_viewCache.TryGetValue(value, out var view))
        {
            var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
            viewModel.Session = value;
            view = new ChatMessageView(viewModel);
            view.Opacity = 0;
            view.FadeTo(1);
            _viewCache.Add(value, view);
        }

        Content = view;
    }

    [ObservableProperty]
    private MessageModel _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageViewModel(
        DataCenter dataCenter,
        ChatHub chatHub,
        ILogger<MessageViewModel> logger)
    {
        DataCenter = dataCenter;
        _chatHub = chatHub;
        _viewCache = new();
        _ = UpdateMessage();
        _logger = logger;
    }

    [RelayCommand]
    private void Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var message = DataCenter.SendText(CurrentSelectedRoom, Message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "send message error.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenNewWindow(RoomModelBase session)
    {
        ServiceHelper.GetService<IWindowManagerService>().OpenChatRoomWindow(session);
        if (CurrentSelectedRoom == session)
        {
            CurrentSelectedRoom = null;
        }
    }

    [RelayCommand]
    private Task Search(string searchText)
    {
        return Toast.Make("Search:" + searchText).Show();
    }

    [RelayCommand]
    private Task JoinDetail(RoomModelBase room)
    {
        if (MauiAppConsts.IsDesktop)
        {
            CurrentSelectedRoom = room;
            if (!_viewCache.TryGetValue(room, out var view))
            {
                var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
                viewModel.Session = room;
                view = new ChatMessageView(viewModel);
                _viewCache.Add(room, view);
            }
            Content = view;
            return Task.CompletedTask;
        }
        else
        {
            return _navigationService.GoToMessageDetailPage(room.Id);
        }
    }

    [RelayCommand]
    private Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    private Task JoinQueryPage()
    {
        if (MauiAppConsts.IsDesktop)
        {
            ServiceHelper.GetService<IWindowManagerService>()?.OpenQueryWindow();
            return Task.CompletedTask;
        }
        else
        {
            return _navigationService.GoToScanningPage();
        }
    }

    [RelayCommand]
    private async Task UpdateMessage()
    {
        try
        {
            await DataCenter.GetUnreadMessageAsync();
            await _chatHub.Connect();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "update messages error.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }
}