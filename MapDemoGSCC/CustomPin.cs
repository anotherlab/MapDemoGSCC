namespace MapDemoGSCC;

// The custom map pin code was derived from a project created by Vladislav Antonyuk
// His code used ImageSource, my code uses resource id's on Android, image bundles on iOS
// https://vladislavantonyuk.azurewebsites.net/articles/Customize-map-pins-in-.NET-MAUI
// https://github.com/VladislavAntonyuk/MauiSamples/tree/main/MauiMaps

using Microsoft.Maui.Controls.Maps;

public class CustomPin : Pin
{
    public static readonly BindableProperty ImageNameProperty =
        BindableProperty.Create(nameof(ImageName), typeof(string), typeof(CustomPin), propertyChanged: OnImageNameChanged);

    // Use for Android
    public int ResourceID { get; set; }

    // Use for iOS / macOS
    public string? ImageName
    {
        get => (string?)GetValue(ImageNameProperty);
        set => SetValue(ImageNameProperty, value);
    }

    public Microsoft.Maui.Maps.IMap? Map { get; set; }

    static async void OnImageNameChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomPin)bindable;
        var imageName = control.ImageName;

        if (control.Handler?.PlatformView is null)
        {
            // Workaround for when this executes the Handler and PlatformView is null
            control.HandlerChanged += OnHandlerChanged;
            return;
        }

#if IOS || MACCATALYST
        await control.AddAnnotation();
#else
        await Task.CompletedTask;
#endif

        void OnHandlerChanged(object? s, EventArgs e)
        {
            OnImageNameChanged(control, oldValue, newValue);
            control.HandlerChanged -= OnHandlerChanged;
        }
    }
}