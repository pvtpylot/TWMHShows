namespace MauiBlazorWeb;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

#if ANDROID
            this.Loaded += (s, e) =>
            {
                var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
                if (activity != null)
                {
                    var resourceId = activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
                    var statusBarHeight = resourceId > 0 ? activity.Resources.GetDimensionPixelSize(resourceId) : 0;
                    // Convert from pixels to device-independent units
                    var density = activity.Resources.DisplayMetrics.Density;
                    this.Padding = new Thickness(0, statusBarHeight / density, 0, 0);
                }
            };
#endif
    }
}