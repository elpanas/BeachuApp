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
                        await DisplayAlert("Errore", "Aggiornamento fallito", "Ok");
                    else
                    {
                        await DisplayAlert("Complimenti", "Aggiornamento effettuato", "Ok");
                        await Navigation.PopAsync();
                    }
                }
                else
                    await DisplayAlert("Errore", "Errore di connessione", "Ok");
            }
            catch
            {
                await DisplayAlert("Errore", "Uno o più campi non contengono un valore accettabile", "Ok");
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