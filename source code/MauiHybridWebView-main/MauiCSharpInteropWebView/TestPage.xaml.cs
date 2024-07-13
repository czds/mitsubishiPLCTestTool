using MauiCSharpInteropWebView.ViewModel;
namespace MauiCSharpInteropWebView;

public partial class TestPage : ContentPage
{
    public TestPage(TestViewModel vm)
    {
        InitializeComponent();

        //BindingContext=new TestViewModel();

        BindingContext = vm; //MauiProgram ¥¶◊¢≤·“≥√Ê“¿¿µ

    }
}