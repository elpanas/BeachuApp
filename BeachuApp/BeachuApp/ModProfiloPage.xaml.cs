using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModProfiloPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();

        public ModProfiloPage()
        {
            InitializeComponent();
            InizializzaCampi();
        }

        async private void InizializzaCampi()
        {
            nome.Text = await SecureStorage.GetAsync("beachunome");
            cognome.Text = await SecureStorage.GetAsync("beachucognome");
            telefono.Text = await SecureStorage.GetAsync("beachutel");
            email.Text = await SecureStorage.GetAsync("beachumail");
        }

        async private void Salva_Clicked(object sender, EventArgs e)
        {
            try
            {
                var beachuid = await SecureStorage.GetAsync("beachuid");

                Dictionary<string, string> parametri = new Dictionary<string, string>()
                {
                    { "id", beachuid },
                    { "nome", nome.Text },
                    { "cognome", cognome.Text },
                    { "telefono", telefono.Text },
                    { "mail", email.Text },
                    { "azione", "aggiornautente" }
                };

                var response = InviaRichiesta(parametri);

                if (!response.IsFaulted)
                {
                    if (JsonConvert.DeserializeObject<int>(response.Result) == 0)
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
                    else
                    {
                        await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");
                        await Navigation.PopAsync();
                    }
                }
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }

        async private Task<string> InviaRichiesta(Dictionary<string, string> parametri)
        {
            string datiJson = JsonConvert.SerializeObject(parametri);
            var response = _client.PostAsync(Url, new StringContent(datiJson));
            return await response.Result.Content.ReadAsStringAsync();
        }
    }
}