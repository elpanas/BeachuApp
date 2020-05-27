using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

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
                    { "nome", nome.Text },
                    { "cognome", cognome.Text },
                    { "email", email.Text},
                    { "username", Convert.ToBase64String(Encoding.UTF8.GetBytes(username.Text)) },
                    { "password", Convert.ToBase64String(Encoding.UTF8.GetBytes(password.Text)) }
                };

                loader.IsRunning = true;
                loader.IsVisible = true;

                var response = await _client.PostAsync(Variabili.UrlUser,
                                                       new StringContent(JsonConvert.SerializeObject(datipersonali),
                                                                         Encoding.UTF8,
                                                                         "application/json"));

                loader.IsRunning = false;
                loader.IsVisible = false;

                if (response.IsSuccessStatusCode)
                {
                    var restring = await response.Content.ReadAsStringAsync();
                    var idu = JsonConvert.DeserializeObject(restring).ToString();
                    await SecureStorage.SetAsync("beachuid", idu);
                    await SecureStorage.SetAsync("beachunome", nome.Text);
                    await SecureStorage.SetAsync("beachucognome", cognome.Text);
                    await SecureStorage.SetAsync("beachusername", username.Text);
                    await SecureStorage.SetAsync("beachumail", email.Text);
                    await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");
                    await Navigation.PopToRootAsync();
                }
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }
    }
}