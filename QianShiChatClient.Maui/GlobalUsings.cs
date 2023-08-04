#if ANDROID

global using QianShiChatClient.Maui.Platforms.Android;

#elif WINDOWS

global using QianShiChatClient.Maui.Platforms.Windows;

#endif

global using Bogus;

global using CommunityToolkit.Maui;
global using CommunityToolkit.Maui.Alerts;
global using CommunityToolkit.Maui.Core;
global using CommunityToolkit.Maui.Converters;
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;

global using Microsoft.AspNetCore.SignalR.Client;
global using Microsoft.Extensions.Localization;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Caching.Memory;
global using Microsoft.Maui.LifecycleEvents;

global using Mopups.Hosting;
global using Mopups.Interfaces;
global using Mopups.Services;

global using QianShiChatClient.Maui.Data;
global using QianShiChatClient.Maui.Extensions;
global using QianShiChatClient.Maui.Helpers;
global using QianShiChatClient.Maui.Models;
global using QianShiChatClient.Maui.Resources.Strings;
global using QianShiChatClient.Maui.Services;
global using QianShiChatClient.Maui.ViewModels;
global using QianShiChatClient.Maui.Views;
global using QianShiChatClient.Maui.Views.Desktop;
global using QianShiChatClient.Maui.Views.Popups;

global using SQLite;

global using SkiaSharp.Views.Maui.Controls.Hosting;

global using System;
global using System.ComponentModel;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.ComponentModel.DataAnnotations;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Linq;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Runtime.CompilerServices;
global using System.Security.Cryptography;
global using System.Text;
global using System.Text.Json;
global using System.Threading.Tasks;
global using System.Web;
global using System.Windows.Input;

global using ZXing.Net.Maui;
global using ZXing.Net.Maui.Controls;

global using Maui.FixesAndWorkarounds;