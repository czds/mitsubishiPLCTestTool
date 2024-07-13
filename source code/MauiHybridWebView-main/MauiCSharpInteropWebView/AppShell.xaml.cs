namespace MauiCSharpInteropWebView;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(TestPage), typeof(TestPage));

        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));

    }
}
