using Android.Views;
using Microsoft.Maui.Platform;

namespace QianShiChatClient.Application;

// All the code in this file is only included on Android.
public static class CursorExtensions
{
    public static void SetCustomCursor(this VisualElement visualElement, CursorIcon cursor, IMauiContext? mauiContext)
    {
        if (OperatingSystem.IsAndroidVersionAtLeast(24))
        {
            ArgumentNullException.ThrowIfNull(mauiContext);
            var view = visualElement.ToPlatform(mauiContext);
            view.PointerIcon = PointerIcon.GetSystemIcon(Android.App.Application.Context, GetCursor(cursor));
        }
    }

    private static PointerIconType GetCursor(CursorIcon cursor)
    {
        return cursor switch
        {
            CursorIcon.Hand => PointerIconType.Hand,
            CursorIcon.IBeam => PointerIconType.AllScroll,
            CursorIcon.Cross => PointerIconType.Crosshair,
            CursorIcon.Arrow => PointerIconType.Arrow,
            CursorIcon.SizeAll => PointerIconType.TopRightDiagonalDoubleArrow,
            CursorIcon.Wait => PointerIconType.Wait,
            _ => PointerIconType.Default,
        };
    }
}