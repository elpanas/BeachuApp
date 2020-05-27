using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StabDispPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();
        private ObservableCollection<Stabilimento> _stabilimenti;

        public StabDispPage()
        {
            InitializeComponent();
            PopolaListaAsync();
        }

        private async void PopolaListaAsync(string cerca = null)
        {
            var app = Application.Current as App;
            string urlbase = Variabili.UrlStab + "disp/";
            string url = urlbase;

            try
            {
                if (app.Properties["cerca"].ToString() == "localita")
                {
                    string loc = app.Properties["localita"].ToString();
                    string prov = app.Properties["provincia"].ToString();
                    localita.Text = app.Properties["localita"].ToString();

                    url = urlbase + "location/" + loc + "/" + prov;
                }
                else if (app.Properties["cerca"].ToString() == "posizione")
                {
                    string lat, longi;

                    if (cerca == null)
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();

                        if (location == null)
                        {
                            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                            location = await Geolocation.GetLocationAsync(request);
                        }

                        if (location != null)
                        {
                            lat = location.Latitude.ToString();
                            longi = location.Longitude.ToString();
                            app.Properties["latitudine"] = location.Latitude;
                            app.Properties["longitudine"] = location.Longitude;
                        }
                        else
                            throw new Exception(AppResources.ErrorCoord);
                    }
                    else
                    {
                        lat = app.Properties["latitudine"].ToString();
                        longi = app.Properties["longitudine"].ToString();
                    }

                    url = urlbase + "coord/" + longi + "/" + lat;
                }

                var response = await _client.GetAsync(url);

                loader.IsRunning = false;
                loader.IsVisible = false;

                if (response.IsSuccessStatusCode)
                {
                    string resstring = await response.Content.ReadAsStringAsync();
                    var stabilimenti = JsonConvert.DeserializeObject<List<Stabilimento>>(resstring);
                    _stabilimenti = new ObservableCollection<Stabilimento>(stabilimenti);

                    if (_stabilimenti.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(cerca))
                            listView.ItemsSource = _stabilimenti;
                        else
                            listView.ItemsSource = _stabilimenti.Where(c => c.nome.StartsWith(cerca));

                        labelvuota.IsVisible = false;
                        listView.IsVisible = true;
                        search.IsVisible = true;
                        localita.IsVisible = true;
                    }
                }
                else
                {
                    loader.IsRunning = false;
                    loader.IsVisible = false;
                    labelvuota.IsVisible = true;
                }
            }
            catch
            {
                loader.IsRunning = false;
                labelvuota.IsVisible = true;
                loader.IsVisible = false;
            }
        }

        private void ListView_Refreshing(object sender, System.EventArgs e)
        {
            PopolaListaAsync();
            listView.EndRefresh();
        }

        async private void Stab_Tapped(object sender, System.EventArgs e)
        {
            var app = Application.Current as App;
            var stabilimento = (sender as TextCell).CommandParameter as Stabilimento;

            // memorizza i dati per utilizzarli in altre pagine
            app.Properties["stabId"] = stabilimento._id;
            app.Properties["stabNome"] = stabilimento.nome;
            app.Properties["stabOmbrelloni"] = stabilimento.ombrelloni;
            app.Properties["stabDisponibili"] = stabilimento.disponibili;
            app.Properties["stabLongitudine"] = stabilimento.location.coordinates[0];
            app.Properties["stabLatitudine"] = stabilimento.location.coordinates[1];
            app.Properties["stabTelefono"] = stabilimento.telefono;

            await Navigation.PushAsync(new StabPageUtente());
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopolaListaAsync(e.NewTextValue);
        }
    }
}