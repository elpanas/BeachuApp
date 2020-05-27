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
    public partial class StabPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();

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

        async private void CaricaGPS()
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
            CaricaGPS();
        }

        async private void Salva_Clicked(object sender, EventArgs e)
        {
            var app = Application.Current as App;
            Dictionary<string, string> parametri = new Dictionary<string, string>();

            try
            {
                parametri["nome"] = nomestab.Text;
                parametri["localita"] = localita.Text;
                parametri["provincia"] = provincia.Text;
                parametri["ombrelloni"] = ombrelloni.Text;
                parametri["idu"] = await SecureStorage.GetAsync("beachuid");
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

            HttpResponseMessage response;

            string idu = await SecureStorage.GetAsync("beachuid");

            loader.IsRunning = true;
            loader.IsVisible = true;

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

            try
            {
                response = await _client.PutAsync(Variabili.UrlStab + app.Properties["stabId"],
                                                  new StringContent(JsonConvert.SerializeObject(parametri),
                                                  Encoding.UTF8,
                                                  "application/json"));
            }
            catch
            {
                response = await _client.PostAsync(Variabili.UrlStab,
                                                   new StringContent(JsonConvert.SerializeObject(parametri),
                                                   Encoding.UTF8,
                                                   "application/json"));
            }

            loader.IsRunning = false;
            loader.IsVisible = false;

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert(AppResources.MsgTitle, AppResources.MsgOperation, "Ok");

                // elimino la pagina precedente...
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);

                // ... in modo da eliminare questa e saltare a ProfiloPage, liberando lo stack
                await Navigation.PopAsync();
            }
            else
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
        }
    }
}