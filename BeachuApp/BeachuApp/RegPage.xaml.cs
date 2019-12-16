using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com/";
        private HttpClient _client = new HttpClient();

        public RegPage()
        {
            InitializeComponent();
        }

        async private void Reg_Clicked(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> datipersonali = new Dictionary<string, string>()
                {
                    { "azione", "inserisciutente" },
                    { "nome", nome.Text },
                    { "cognome", cognome.Text },
                    { "username", Convert.ToBase64String(Encoding.UTF8.GetBytes(username.Text)) },
                    { "password", Convert.ToBase64String(Encoding.UTF8.GetBytes(password.Text)) }
                };

                var response = InviaRichiesta(datipersonali);

                if (!response.IsFaulted)
                {
                    var esito = JsonConvert.DeserializeObject<string>(response.Result);

                    if (esito == "1")
                    {
                        await SecureStorage.SetAsync("beachunome", nome.Text);
                        await SecureStorage.SetAsync("beachucognome", cognome.Text);
                        await DisplayAlert("Congratulazioni!", "Ora sei registrato!", "Ok");
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await DisplayAlert("Errore", "Operazione fallita", "Ok");
                    }
                }
            }
            catch
            {
                await DisplayAlert("Errore", "Alcuni campi non contengono valori accettabili: ", "Ok");
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