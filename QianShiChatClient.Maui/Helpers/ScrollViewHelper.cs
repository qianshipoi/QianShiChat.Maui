namespace QianShiChatClient.Maui.Helpers;

public static class ScrollViewHelper
{
    #region ScrollToPosition

    public static ScrollToPosition GetScrollToPosition(BindableObject obj)
    {
        return (ScrollToPosition)obj.GetValue(ScrollToPositionProperty);
    }

    public static void SetScrollItem(BindableObject obj, ScrollToPosition value)
    {
        obj.SetValue(ScrollToPositionProperty, value);
    }

    public static readonly BindableProperty ScrollToPositionProperty =
        BindableProperty.CreateAttached(
            "ScrollToPosition",
            typeof(ScrollToPosition),
            typeof(ScrollViewHelper),
            ScrollToPosition.MakeVisible);

    #endregion

    #region ScrollAnimated

    public static bool GetScrollAnimated(BindableObject obj)
    {
        return (bool)obj.GetValue(ScrollAnimatedProperty);
    }

    public static void SetScrollAnimated(BindableObject obj, bool value)
    {
        obj.SetValue(ScrollAnimatedProperty, value);
    }

    public static readonly BindableProperty ScrollAnimatedProperty =
        BindableProperty.CreateAttached(
            "ScrollAnimated",
            typeof(bool),
            typeof(ScrollViewHelper),
            true);

    #endregion

    #region ScrollCurrentItem

    public static object GetScrollCurrentItem(BindableObject obj)
    {
        return (object)obj.GetValue(ScrollCurrentItemProperty);
    }

    public static void SetScrollCurrentItem(BindableObject obj, object value)
    {
        obj.SetValue(ScrollCurrentItemProperty, value);
    }

    public static readonly BindableProperty ScrollCurrentItemProperty =
        BindableProperty.CreateAttached(
            "ScrollCurrentItem",
            typeof(object),
            typeof(ScrollViewHelper),
            null,
             propertyChanged: OnScrollItemChanged);

    static void OnScrollItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (newValue == null ||
            bindable is not ScrollView scrollView ||
            scrollView.Content is not Layout layout)
        {
            return;
        }

        foreach (var child in layout.Children)
        {
            if (child is Element element)
            {
                scrollView.ScrollToAsync(
                    element, 
                    GetScrollToPosition(bindable),
                    GetScrollAnimated(bindable));
            }
        }
    }

    #endregion
}
