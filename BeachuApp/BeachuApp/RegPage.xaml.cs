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
                        await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");
                        await Navigation.PopToRootAsync();
                    }
                    else
                    {
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
                    }
                }
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