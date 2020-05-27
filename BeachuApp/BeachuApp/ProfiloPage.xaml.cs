using BeachuApp.Resx;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfiloPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

        public ProfiloPage()
        {
            InitializeComponent();
            InizializzaCampi();
        }

        async private void InizializzaCampi()
        {
            try
            {
                nome.Text = await SecureStorage.GetAsync("beachusername");
                Navigation.RemovePage(new LoginPage());
            }
            catch
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        async private void Stabilimenti_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ElencoStabPage());
        }

        async private void Modifica_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ModProfiloPage());
        }

        async private void Elimina_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert(AppResources.DeleteAccountLabel,
                                             AppResources.ConfirmText,
                                             AppResources.ConfirmYes,
                                             AppResources.ConfirmNo);

            if (answer)
            {
                var idu = await SecureStorage.GetAsync("beachuid");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                                                                                            Funzioni.CodificaId(idu));

                var response = await _client.DeleteAsync(Variabili.UrlUser + idu);

                if (response.IsSuccessStatusCode)
                {
                    SecureStorage.RemoveAll();
                    await Navigation.PopToRootAsync();
                }
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorConn, "Ok");
            }
        }
    }
}