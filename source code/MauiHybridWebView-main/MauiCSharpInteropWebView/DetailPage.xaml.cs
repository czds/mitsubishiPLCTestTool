using MauiCSharpInteropWebView.ViewModel;

namespace MauiCSharpInteropWebView;

public partial class DetailPage : ContentPage
{
    public DetailPage(DetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}