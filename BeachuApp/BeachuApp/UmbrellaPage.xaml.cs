using BeachuApp.Resx;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UmbrellaPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

        public UmbrellaPage()
        {
            InitializeComponent();
            InitStabilimento();
        }

        async private void InitStabilimento()
        {
            try
            {
                loader.IsRunning = true;
                loader.IsVisible = true;

                string idu = await SecureStorage.GetAsync("beachuid");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

                var response = await _client.GetAsync(Variabili.UrlStab + Application.Current.Properties["stabId"]);

                loader.IsRunning = false;
                loader.IsVisible = false;

                if (response.IsSuccessStatusCode)
                {
                    var resstring = await response.Content.ReadAsStringAsync();
                    var stabilimento = JsonConvert.DeserializeObject<List<Stabilimento>>(resstring);

                    nome.Text = stabilimento[0].nome;
                    stepper.Maximum = stabilimento[0].ombrelloni;
                    stepper.Value = stabilimento[0].disponibili;
                    ombrelloni.Text = AppResources.Available + stabilimento[0].disponibili;
                }
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
            }
        }

        async private void Ombrelloni_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Dictionary<string, string> parametri = new Dictionary<string, string>()
            {
                { "ids", Application.Current.Properties["stabId"].ToString() },
                { "ombrelloni", e.NewValue.ToString() }
            };

            string idu = await SecureStorage.GetAsync("beachuid");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

            var response = await _client.PutAsync(Variabili.UrlStab + "disp",
                                                  new StringContent(JsonConvert.SerializeObject(parametri),
                                                  Encoding.UTF8,
                                                  "application/json"));

            if (response.IsSuccessStatusCode)
                ombrelloni.Text = AppResources.Available + stepper.Value;
            else
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
        }
    }
}