using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

using System.Windows.Input;

namespace WPF_CurrencyConverter_API
{
    public partial class MainWindow : Window
    {
        Root values = new Root();
        // Root Class is a Main Class
        // API Return rates, All Currency with Name and Value
        public class Root
        {
            public Rate rates { get; set; } // get All record in rates and Set in Rate Class as Currency Name
            public long timestamp;
            public string license;
            public string disclaimer;
        }
        public class Rate // MAKE SURE API return VALUE NAME AND WHERE YOU WANT TO STORE NAME ARE THE SAME
        {
            public double USD { get; set; }
            public double MXN { get; set; }
            public double AUD { get; set; }
            public double ARS { get; set; }
            public double PEN { get; set; }
            public double EUR { get; set; }
            public double CNY { get; set; }
            public double KRW { get; set; }
            public double COP { get; set; }
            public double CAD { get; set; }
            public double CLP { get; set; }
            public double JPY { get; set; }
            public double GBP { get; set; }
            public double BTC { get; set; }
            public double INR { get; set; }
        }
        // ----- Main Method App -----
        public MainWindow()
        {
            InitializeComponent();
            ResetControls();
            GetValues();
        }
        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                using (var client = new HttpClient()) // HttpClient class provides a base class for sending/receiving the HTTP requests/responses from a URL
                {
                    client.Timeout = TimeSpan.FromSeconds(30); // The timespan to wait before the request times out
                    HttpResponseMessage response = await client.GetAsync(url); // HttpResponseMessage is a way of returning a message/data from your action
                    if (response.StatusCode == System.Net.HttpStatusCode.OK) // Check API response status code OK
                    {
                        // Convert the received information from byte to string in order that our program can read it, so to speak
                        var responseString = await response.Content.ReadAsStringAsync(); // Serialize the HTTP content to a string as an asynchronous operation
                        // Convert the string in JSON format
                        var responseObject = JsonConvert.DeserializeObject<Root>(responseString); // JsonConvert.DeserializeObject to deserialize Json to a C#

                        MessageBox.Show("License: " + responseObject.license + "\n" + responseObject.disclaimer, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        return responseObject; // return
                    }
                    return myRoot;
                }
            }
            catch (TaskCanceledException)
            {
                MessageBox.Show("The request timed out. Please check your connection and try again.", "Timeout", MessageBoxButton.OK, MessageBoxImage.Warning);
                return myRoot;
            }
            catch
            {
                MessageBox.Show("An unexpected error occurred. Please try again later or contact support", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return myRoot;
            }
        }
        private async void GetValues()
        {
            // Set here the api url
            values = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=YOUR_API_KEY_HERE");
            BindCurrency();
        }
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();

            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            dtCurrency.Rows.Add("--SELECT--", 0);
            dtCurrency.Rows.Add("United States Dollar - USD", values.rates.USD);
            dtCurrency.Rows.Add("European Union - EUR", values.rates.EUR);
            dtCurrency.Rows.Add("Colombian Peso - COP", values.rates.COP);
            dtCurrency.Rows.Add("Mexican Peso - MXN", values.rates.MXN);
            dtCurrency.Rows.Add("Bitcoin - BTC", values.rates.BTC);
            dtCurrency.Rows.Add("Great Britain Pound - GBP", values.rates.GBP);
            dtCurrency.Rows.Add("Australian Dollar - AUD", values.rates.AUD);
            dtCurrency.Rows.Add("Chinese Yuan - CNY", values.rates.CNY);
            dtCurrency.Rows.Add("South Korean Won - KRW", values.rates.KRW);
            dtCurrency.Rows.Add("Japanese Yen - JPY", values.rates.JPY);
            dtCurrency.Rows.Add("Argentine Peso - ARS", values.rates.ARS);
            dtCurrency.Rows.Add("Peruvian Sol - PEN", values.rates.PEN);
            dtCurrency.Rows.Add("Chilean Peso - CLP", values.rates.CLP);
            dtCurrency.Rows.Add("Canadian Dollar - CAD", values.rates.CAD);
            dtCurrency.Rows.Add("Indian Rupee - INR", values.rates.INR);

            FromCurrency.ItemsSource = dtCurrency.DefaultView;
            FromCurrency.DisplayMemberPath = "Text";
            FromCurrency.SelectedValuePath = "Value";
            FromCurrency.SelectedIndex = 0;

            ToCurrency.ItemsSource = dtCurrency.DefaultView;
            ToCurrency.DisplayMemberPath = "Text";
            ToCurrency.SelectedValuePath = "Value";
            ToCurrency.SelectedIndex = 0;

            CurrencyToday.ItemsSource = dtCurrency.DefaultView;
            CurrencyToday.DisplayMemberPath = "Text";
            CurrencyToday.SelectedValuePath = "Value";
            CurrencyToday.SelectedIndex = 0;
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double convertedValue;
            // cover exceptions
            if (AmountBox == null || AmountBox.Text.Trim() == "")
            {
                MessageBox.Show("Please Enter Currency", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                AmountBox.Focus();
                return;
            }
            else if (FromCurrency.SelectedValue == null || FromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a currency from", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                FromCurrency.Focus();
                return;
            }
            else if (ToCurrency == null || ToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select a currency to", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                ToCurrency.Focus();
                return;
            }
            // If both currencies are same
            if (FromCurrency.Text == ToCurrency.Text)
            {
                if (FromCurrency.SelectedIndex != 0 && ToCurrency.SelectedIndex == 0)
                {
                    convertedValue = double.Parse(AmountBox.Text);
                    LabelConvertedValue.Content = ToCurrency.Text + " " + convertedValue;
                }
            }
            else // -------------- Here is de Actual calculation ------------------
            {
                if (FromCurrency.SelectedIndex != 0 && ToCurrency.SelectedIndex != 0)
                {
                    convertedValue = (double.Parse(AmountBox.Text)) / (double.Parse(FromCurrency.SelectedValue.ToString())) * (double.Parse(ToCurrency.SelectedValue.ToString()));
                    LabelConvertedValue.Content = ToCurrency.Text + " " + convertedValue.ToString("N3");
                }
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetControls();
        }
        private void EnteredAmount(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumber(e.Text))
            {
                e.Handled = true;
            }
        }
        private bool IsNumber(string e)
        {
            if (int.TryParse(e, out int i))
            {
                return true;
            }
            else return false;
        }
        private void ResetControls()
        {
            AmountBox.Text = string.Empty;
            if (FromCurrency.Items.Count > 0)
            {
                FromCurrency.SelectedIndex = 0;
            }
            if (ToCurrency.Items.Count > 0)
            {
                ToCurrency.SelectedIndex = 0;
            }
            if (CurrencyToday.Items.Count > 0)
            {
                CurrencyToday.SelectedIndex = 0;
            }
            LabelConvertedValue.Content = string.Empty;
            CurrencyToday_InfoLabel.Content = string.Empty;
            CurrencyToday_ValueLabel.Content = string.Empty;
            AmountBox.Focus();
        }

        private void CurrencyToday_Value(object sender, SelectionChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var selectedValue = CurrencyToday.SelectedValue?.ToString();
                if (!string.IsNullOrEmpty(selectedValue) && CurrencyToday.SelectedIndex != 0)
                {
                    CurrencyToday_ValueLabel.Content = selectedValue;
                    CurrencyToday_InfoLabel.Content = CurrencyToday.Text;
                }
            }));
        }
    }
}