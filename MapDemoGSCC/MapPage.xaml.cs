namespace MapDemoGSCC;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;

public partial class MapPage : ContentPage
{
    Location tylerLocation;
    Location gsccLocation;
    Location homeLocation;
    public MapPage()
	{
		InitializeComponent();
        tylerLocation = new Location(42.756237114785705, -73.82287033453463);

        gsccLocation = new Location(43.01904, -71.48381);

        homeLocation = gsccLocation;

        #region geo api
        map.MapClicked += async (sender, e) =>
        {
            var info = await ReverseGeoCoding(e.Location);
            await DisplayAlert("Placemark", info, "Ok");
        };
        #endregion
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var mapSpan = MapSpan.FromCenterAndRadius(homeLocation, Distance.FromKilometers(3));

        map.MoveToRegion(mapSpan);

        AddHomePin();
    }

    private void AddHomePin()
    {
        map.Pins.Clear();

        //var homePin = new Pin()
        //{
        //    Label = "GSCC",
        //    Location = homeLocation,
        //    Address = "Manchester Community College",
        //};

        var homePin = new CustomPin()
        {
            Label = "GSCC",
            Location = homeLocation,
            Address = "Manchester Community College",
            Map = map
        };

#if ANDROID
        homePin.ResourceID = Resource.Drawable.gscc_circle;
#elif IOS || MACCATALYST
        homePin.ImageName = "gscc_circle.png";
#endif

        map.Pins.Add(homePin);

    }

    #region Geo API
    private void Shapes_Clicked(object sender, EventArgs e)
    {
        Polygon polygon = new Polygon
        {
            StrokeWidth = 4,
            StrokeColor = Color.FromArgb("#1BA1E2"),
            FillColor = Color.FromArgb("#881BA1E2"),
            Geopath =
            {
                new Location(43.0187646, -71.4846777),
                new Location(43.017235,  -71.4831005),
                new Location(43.0181292, -71.4812122),
                new Location(43.0191333, -71.4809548),
                new Location(43.0202471, -71.4814483),
                new Location(43.0213061, -71.4820813),
                new Location(43.0212041, -71.4842807),
                new Location(43.0200118, -71.4860509),
                new Location(43.0187646, -71.4846777),
            }
        };

        Circle circle = new Circle
        {
            Center = homeLocation,
            Radius = Distance.FromKilometers(0.5),
            FillColor = Color.FromArgb("#5519fc56")
        };

        AddHomePin();
        map.MapElements.Clear();
        map.MapElements.Add(polygon);
        map.MapElements.Add(circle);
    }
    public async void DoSearch(string address)
    {
        try
        {
            var locations = await Geocoding.GetLocationsAsync(address);

            var location = locations?.FirstOrDefault();

            if (location != null)
            {
                var mapspan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(1.0));

                map.MoveToRegion(mapspan);
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Geocoding error: {ex.Message}");
        }
    }
    private void Route_Clicked(object sender, EventArgs e)
    {
        Polyline route = new Polyline
        {
            StrokeColor = Color.FromArgb("#FF0000FF"),
            StrokeWidth = 8,
            Geopath =
            {
                new Location(43.01909, -71.48373),
                new Location(43.01893, -71.48352),
                new Location(43.01894, -71.48344),
                new Location(43.01902, -71.48333),
                new Location(43.01916, -71.48313),
                new Location(43.01885, -71.48282),
                new Location(43.01908, -71.48248),
                new Location(43.01921, -71.48241),
                new Location(43.01934, -71.48238),
                new Location(43.01958, -71.48235),
                new Location(43.0197, -71.48228),
                new Location(43.01979, -71.48218),
                new Location(43.01984, -71.48205),
                new Location(43.01983, -71.48173),
                new Location(43.01979, -71.48157),
                new Location(43.01969, -71.48144),
                new Location(43.01956, -71.48138),
                new Location(43.01921, -71.4813),
                new Location(43.01908, -71.48123),
                new Location(43.01901, -71.48116),
                new Location(43.01896, -71.48093),
                new Location(43.01698, -71.48116),
                new Location(43.01678, -71.48117),
                new Location(43.01554, -71.48131),
                new Location(43.01511, -71.48134),
                new Location(43.01498, -71.48133),
                new Location(43.01492, -71.48132),
                new Location(43.01463, -71.48123),
                new Location(43.01435, -71.48107),
                new Location(43.01408, -71.48083),
                new Location(43.0138, -71.48048),
                new Location(43.01367, -71.48028),
                new Location(43.01334, -71.48008),
                new Location(43.01306, -71.47994),
                new Location(43.01215, -71.47952),
                new Location(43.01167, -71.47931),
            }
        };

        var corner1 = new Location(43.01984, -71.47931);
        var corner2 = new Location(43.01167, -71.483734);
        var center = new Location((corner1.Latitude + corner2.Latitude) / 2.0, (corner1.Longitude + corner2.Longitude) / 2.0);
        var distance = Location.CalculateDistance(corner1, corner2, DistanceUnits.Kilometers) * 0.5;

        var mapSpan = MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(distance));

        map.MoveToRegion(mapSpan);

        AddHomePin();

        map.Pins.Add(new Pin
        {
            Label = "Destination",
            Type = PinType.SearchResult,
            Location = route.Geopath[route.Geopath.Count - 1]
        });

        map.MapElements.Clear();
        map.MapElements.Add(route);
    }

    private void SearchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        DoSearch(searchBar.Text);
    }

    public async Task<string> ReverseGeoCoding(Location location)
    {
        string geocodeAddress = string.Empty;

        try
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);

            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                geocodeAddress =
                    $"Latitude:        {location.Latitude}\n" +
                    $"Longitude:       {location.Longitude}\n" +
                    $"AdminArea:       {placemark.AdminArea}\n" +
                    $"CountryCode:     {placemark.CountryCode}\n" +
                    $"CountryName:     {placemark.CountryName}\n" +
                    $"FeatureName:     {placemark.FeatureName}\n" +
                    $"Locality:        {placemark.Locality}\n" +
                    $"PostalCode:      {placemark.PostalCode}\n" +
                    $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                    $"SubLocality:     {placemark.SubLocality}\n" +
                    $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                    $"Thoroughfare:    {placemark.Thoroughfare}\n";
            }
        }
        catch (Exception ex)
        {
            geocodeAddress = $"Error: {ex.Message}";
        }

        return geocodeAddress;
    }
#endregion
}