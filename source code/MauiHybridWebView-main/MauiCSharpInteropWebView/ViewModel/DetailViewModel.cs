using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiCSharpInteropWebView.Fun;
using System.Web;


namespace MauiCSharpInteropWebView.ViewModel
{
    [QueryProperty("Title", "id")]
    public partial class DetailViewModel : ObservableObject, IQueryAttributable
    {
        string searchString;

        string mbatchno;
        public string Mbatchno
        {
            get => mbatchno;
            set
            {
                mbatchno = value;
                OnPropertyChanged(nameof(Mbatchno));
            }
        }
        string qty;
        public string Qty
        {
            get => qty;
            set
            {
                qty = value;
                OnPropertyChanged(nameof(Qty));
            }
        }

        string dinsert;
        public string Dinsert
        {
            get => dinsert;
            set
            {
                dinsert = value;
                OnPropertyChanged(nameof(Dinsert));
            }
        }


        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            searchString = HttpUtility.UrlDecode(query["name"].ToString());

            if (string.IsNullOrWhiteSpace(searchString)) return;

            AidTool.SqlDataReaderResult res = AidTool.vra_pool_open("SELECT [title],[mbatchno],[qty],[dinsert] FROM [DBXXX].[dbo].[testTitleOnly] where title='" + searchString + "'");

            if (!res.successed) return;

            System.Data.DataTable tb = res.dataTable;

            if (tb == null) return;
            if (tb.Rows.Count <= 0) return;

            //Mbatchno = "批次： "+ tb.Rows[0]["mbatchno"].ToString();
            //Qty = "数量： " + tb.Rows[0]["qty"].ToString();
            //Dinsert = "时间： " + tb.Rows[0]["dinsert"].ToString();

        }




        [ObservableProperty]
        string title;

        [RelayCommand]
        async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");//返回上一页面
        }

    }
}
