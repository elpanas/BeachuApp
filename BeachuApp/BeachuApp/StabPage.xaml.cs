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
    public partial class StabPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();

        public StabPage()
        {
            InitializeComponent();
            PopolaPagina();
        }

        private void PopolaPagina()
        {
            var app = Application.Current as App;

            try
            {
                nomestab.Text = app.Properties["stabNome"].ToString();
                ombrelloni.Text = app.Properties["stabOmbrelloni"].ToString();
                localita.Text = app.Properties["stabLocalita"].ToString();
                provincia.Text = app.Properties["stabProvincia"].ToString();

                if (!String.IsNullOrEmpty(app.Properties["stabTelefono"].ToString()))
                    telefono.Text = app.Properties["stabTelefono"].ToString();

                if (!String.IsNullOrEmpty(app.Properties["stabMail"].ToString()))
                    email.Text = app.Properties["stabMail"].ToString();

                if (!String.IsNullOrEmpty(app.Properties["stabWeb"].ToString()))
                    sitoweb.Text = app.Properties["stabWeb"].ToString();

                nomestab.IsEnabled = false;
                localita.IsEnabled = false;
                provincia.IsEnabled = false;
            }
            catch
            {
                // niente
            }
        }

        async private void caricaGPS()
        {
            var app = Application.Current as App;

            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    app.Properties["lat"] = location.Latitude;
                    app.Properties["long"] = location.Longitude;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorDevice + fnsEx, "Ok"); // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorGps + fneEx, "Ok"); // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPerm + pEx, "Ok"); // Handle permission exception
            }
            catch (Exception ex)
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPosition + ex, "Ok"); // Unable to get location
            }
        }

        private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            caricaGPS();
        }

        async private void Salva_Clicked(object sender, EventArgs e)
        {
            var app = Application.Current as App;
            Dictionary<string, string> parametri = new Dictionary<string, string>();
            try
            {
                parametri["azione"] = "inseriscistabilimento";
                parametri["idu"] = await SecureStorage.GetAsync("beachuid");
                parametri["nome"] = nomestab.Text;
                parametri["ombrelloni"] = ombrelloni.Text;
                parametri["localita"] = localita.Text;
                parametri["provincia"] = provincia.Text;
                parametri["telefono"] = telefono.Text;
                parametri["mail"] = email.Text;
                parametri["web"] = sitoweb.Text;
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }

            try
            {
                parametri["latitudine"] = app.Properties["lat"].ToString();
                parametri["longitudine"] = app.Properties["long"].ToString();

                app.Properties.Remove("lat");
                app.Properties.Remove("long");
            }
            catch
            {
                // niente
            }

            var response = InviaRichiesta(parametri);

            if (!response.IsFaulted)
            {
                if (JsonConvert.DeserializeObject<string>(response.Result) == "1")
                    await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");
                else
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
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