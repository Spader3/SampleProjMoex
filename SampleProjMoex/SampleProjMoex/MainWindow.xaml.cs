using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SampleProjMoex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DownloadData();
        }
        private void DownloadData()
        {
            var request = WebRequest.Create("https://iss.moex.com/iss/analyticalproducts/futoi/securities.json") as HttpWebRequest;
            request.Credentials = new NetworkCredential("iyushirokov@gmail.com", "871117");
            var response = request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            var result = readStream.ReadToEnd();
            Temperatures deserializedProduct = JsonConvert.DeserializeObject<Temperatures>(result);
            ObservableCollection<GridModel> users = new ObservableCollection<GridModel>();
            var uniqe = new List<string>();
            foreach (var obj in deserializedProduct.futoi.Data)
            {
                uniqe.Add(obj[4].ToString());
                users.Add(new GridModel
                {
                    sess_id = obj[0].ToString(),
                    seqnum = obj[1].ToString(),
                    tradedate = obj[2].ToString(),
                    tradetime = obj[3].ToString(),
                    ticker = obj[4].ToString(),
                    clgroup = obj[5].ToString(),
                    pos = obj[6].ToString(),
                    pos_long = obj[7].ToString(),
                    pos_short = obj[8].ToString(),
                    pos_long_num = obj[9].ToString(),
                    pos_short_num = obj[10].ToString(),
                    systime = obj[11].ToString()
                });
                grid.ItemsSource = users;
            }
            var uniqeNew = uniqe.Distinct();
            foreach (var it in uniqeNew)
            {
                var listItem = new ListBoxItem { Content = it };
                list.Items.Add(listItem);
            }
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                 var date1 = calendar.SelectedDates.FirstOrDefault();
                var date2 = calendar.SelectedDates.LastOrDefault();
                string dateInFormatFirst;
                string dateInFormatSecond;
                if (date1 > date2)
                {
                    dateInFormatFirst = date2.ToString("yyyy-MM-dd");
                    dateInFormatSecond = date1.ToString("yyyy-MM-dd");
                }
                else
                {
                    dateInFormatFirst = date1.ToString("yyyy-MM-dd");
                    dateInFormatSecond = date2.ToString("yyyy-MM-dd");
                }
                var request = WebRequest.Create("https://iss.moex.com/iss/analyticalproducts/futoi/securities/" +(list.SelectedItem as ListBoxItem).Content.ToString() +".json?from=" + dateInFormatFirst + "&till=" + dateInFormatSecond) as HttpWebRequest;
                HttpStatusCode last_status;
                string last_status_text = null;
                CookieContainer cookiejar = new CookieContainer();
                var AuthReq = WebRequest.Create("https://passport.moex.com/authenticate") as HttpWebRequest;
                AuthReq.CookieContainer = new CookieContainer();
                HttpWebResponse AuthResponse;
                // use the Basic authorization mechanism
                byte[] binData;
                binData = System.Text.Encoding.UTF8.GetBytes("iyushirokov@gmail.com" + ":" + "871117");
                string sAuth64 = Convert.ToBase64String(binData, System.Base64FormattingOptions.None);
                AuthReq.Headers.Add(HttpRequestHeader.Authorization, "Basic " + sAuth64);

                AuthResponse = (HttpWebResponse)AuthReq.GetResponse();
                AuthResponse.Close();
                last_status = AuthResponse.StatusCode;

                cookiejar = new CookieContainer();
                cookiejar.Add(AuthResponse.Cookies);
                request.CookieContainer = cookiejar;
                var response = request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                var result = readStream.ReadToEnd();
                Temperatures deserializedProduct = JsonConvert.DeserializeObject<Temperatures>(result);
                ObservableCollection<GridModel> users = new ObservableCollection<GridModel>();
                foreach (var obj in deserializedProduct.futoi.Data)
                {
                    users.Add(new GridModel
                    {
                        sess_id = obj[0].ToString(),
                        seqnum = obj[1].ToString(),
                        tradedate = obj[2].ToString(),
                        tradetime = obj[3].ToString(),
                        ticker = obj[4].ToString(),
                        clgroup = obj[5].ToString(),
                        pos = obj[6].ToString(),
                        pos_long = obj[7].ToString(),
                        pos_short = obj[8].ToString(),
                        pos_long_num = obj[9].ToString(),
                        pos_short_num = obj[10].ToString(),
                        systime = obj[11].ToString()
                    });
                    grid1.ItemsSource = null;
                    grid1.ItemsSource = users;
                }
            }
            catch(Exception ex)
            {

            }
        }
    }
    public partial class Temperatures
    {
        public Futoi futoi { get; set; }
    }

    public partial class Futoi
    {
        public Metadata Metadata { get; set; }
        public List<string> Columns { get; set; }
        public List<List<object>> Data { get; set; }
    }

    public partial class Metadata
    {
    }
    public class GridModel
    {
        public string sess_id { set; get; }
        public string seqnum { set; get; }
        public string tradedate { set; get; }
        public string tradetime { set; get; }
        public string ticker { set; get; }
        public string clgroup { set; get; }
        public string pos { set; get; }
        public string pos_long { set; get; }
        public string pos_short { set; get; }
        public string pos_long_num { set; get; }
        public string pos_short_num { set; get; }
        public string systime { set; get; }
    }
}
