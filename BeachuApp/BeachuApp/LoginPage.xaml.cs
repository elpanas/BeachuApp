using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class LoginPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

        public LoginPage()
        {
            InitializeComponent();
        }

        async private void ButtonLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username.Text) || string.IsNullOrWhiteSpace(password.Text))
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
                else
                {
                    var authData = string.Format("{0}:{1}", username.Text, password.Text);
                    var authHeaderValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(authData));

                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

                    loader.IsVisible = true;

                    var response = await _client.GetAsync(Variabili.UrlUser + "login");

                    if (response.IsSuccessStatusCode)
                    {
                        var resstring = await response.Content.ReadAsStringAsync();
                        var idu = JsonConvert.DeserializeObject(resstring).ToString();
                        await SecureStorage.SetAsync("beachuid", idu);
                        await SecureStorage.SetAsync("beachusername", username.Text);
                        await Navigation.PushAsync(new ProfiloPage()); // carica la pagina del profilo                        
                    }
                    else
                    {
                        loader.IsVisible = false;
                        await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
                    }
                }
            }
            catch
            {
                loader.IsVisible = false;
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }

        async private void Reg_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegPage());
        }
    }
}