using BeachuApp.Resx;
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

    public partial class LoginPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();

        public LoginPage()
        {
            InitializeComponent();
        }

        async private void ButtonLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
                }
                else
                {
                    Dictionary<string, string> parametri = new Dictionary<string, string>()
                    {
                        { "azione", "login" },
                        { "username", Convert.ToBase64String(Encoding.UTF8.GetBytes(username.Text)) },
                        { "password", Convert.ToBase64String(Encoding.UTF8.GetBytes(password.Text)) }
                    };

                    var response = InviaRichiesta(parametri);

                    if (response.IsFaulted)
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
                    }
                    else
                    {
                        var datiUtente = JsonConvert.DeserializeObject<Utente>(response.Result); // converte json in un oggetto Utente

                        if (datiUtente.Id > 0)
                        {
                            await SecureStorage.SetAsync("beachuid", datiUtente.Id.ToString());
                            await SecureStorage.SetAsync("beachuusername", username.Text);

                            await Navigation.PushAsync(new ProfiloPage()); // carica la pagina del profilo
                        }
                        else
                        {
                            await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
                        }
                    }
                }
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }

        async private void Reg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegPage());
        }

        async private Task<string> InviaRichiesta(Dictionary<string, string> parametri)
        {
            string datiJson = JsonConvert.SerializeObject(parametri);
            var response = _client.PostAsync(Url, new StringContent(datiJson));
            return await response.Result.Content.ReadAsStringAsync();
        }
    }
}