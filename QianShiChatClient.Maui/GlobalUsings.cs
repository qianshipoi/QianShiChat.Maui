#if ANDROID

global using QianShiChatClient.Maui.Platforms.Android;

#elif WINDOWS

global using QianShiChatClient.Maui.Platforms.Windows;

#endif

global using CommunityToolkit.Maui;
global using CommunityToolkit.Maui.Alerts;
global using CommunityToolkit.Maui.Converters;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using Maui.FixesAndWorkarounds;

global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Maui.LifecycleEvents;

global using Mopups.Hosting;
global using Mopups.Interfaces;
global using Mopups.Services;

global using QianShiChatClient.Application.Data;
global using QianShiChatClient.Application.Extensions;
global using QianShiChatClient.Application.Helpers;
global using QianShiChatClient.Application.Maui;
global using QianShiChatClient.Application.Models;
global using QianShiChatClient.Application.Services;
global using QianShiChatClient.Core;
global using QianShiChatClient.Core.Common;
global using QianShiChatClient.Maui.Resources.Strings;
global using QianShiChatClient.Maui.Services;
global using QianShiChatClient.Maui.ViewModels;
global using QianShiChatClient.Maui.Views;
global using QianShiChatClient.Maui.Views.Desktop;
global using QianShiChatClient.Maui.Views.Popups;

global using SimpleToolkit.Core;
global using SimpleToolkit.SimpleShell;

global using SkiaSharp.Views.Maui.Controls.Hosting;

global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Linq;
global using System.Text;
global using System.Threading.Tasks;
global using System.Windows.Input;

global using ZXing.Net.Maui;
global using ZXing.Net.Maui.Controls;
