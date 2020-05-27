using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
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
    public partial class ModProfiloPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

        public ModProfiloPage()
        {
            InitializeComponent();
            InizializzaCampi();
        }

        async private void InizializzaCampi()
        {
            nome.Text = await SecureStorage.GetAsync("beachunome");
            cognome.Text = await SecureStorage.GetAsync("beachucognome");
            email.Text = await SecureStorage.GetAsync("beachumail");
        }

        async private void Salva_Clicked(object sender, EventArgs e)
        {
            try
            {
                var idu = await SecureStorage.GetAsync("beachuid");

                Dictionary<string, string> parametri = new Dictionary<string, string>()
                {
                    { "email", email.Text }
                };

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

                loader.IsRunning = true;
                loader.IsVisible = true;

                var response = await _client.PutAsync(Variabili.UrlUser + idu,
                                                      new StringContent(JsonConvert.SerializeObject(parametri),
                                                      Encoding.UTF8,
                                                      "application/json"));

                loader.IsRunning = false;
                loader.IsVisible = false;

                if (response.IsSuccessStatusCode)
                {
                    await SecureStorage.SetAsync("beachumail", email.Text);
                    await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");
                    await Navigation.PopAsync();
                }
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }
    }
}