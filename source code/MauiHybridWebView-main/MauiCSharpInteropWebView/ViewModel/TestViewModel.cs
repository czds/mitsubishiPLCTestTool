using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;


namespace MauiCSharpInteropWebView.ViewModel
{
    public partial class TestViewModel : ObservableObject
    {
        public TestViewModel()
        {
            Items = new ObservableCollection<string>();
            Items.Add("Monkey");
        }

        [ObservableProperty]
        ObservableCollection<string> items;

        [ObservableProperty]
        string text;

        [RelayCommand]
        void AddNew()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            Items.Add(Text);
            Text = string.Empty;
        }

        [RelayCommand]
        void Remove(string s)
        {
            if (Items.Contains(s))
            {
                Items.Remove(s);
            }
        }
        [RelayCommand]
        async Task Tap(string s)
        {
            //dynamic obj1 = new System.Dynamic.ExpandoObject();
            //obj1.Name = "地1";

            //dynamic obj2 = new System.Dynamic.ExpandoObject();
            //obj2.Birthday = DateTime.Now;

            //    new Dictionary<string, object>
            //    {
            //        {nameof(DetailPage),  new {Name = "a",Age="31",Birthday =DateTime.Now}},
            //        {nameof(DetailPage), new { Name = "as", Age = "3", Birthday = DateTime.Now }}
            //    }

            await Shell.Current.GoToAsync($"{nameof(DetailPage)}?id={s}&name={s}");
        }

    }
}
