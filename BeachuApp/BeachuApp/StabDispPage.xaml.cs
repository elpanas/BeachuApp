using BeachuApp.Resx;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StabDispPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();
        private ObservableCollection<Stabilimento> _stabilimenti;

        public StabDispPage()
        {
            InitializeComponent();
            PopolaListaAsync();
        }

        private async void PopolaListaAsync(string cerca = null)
        {
            var app = Application.Current as App;
            Dictionary<string, string> parametri = new Dictionary<string, string>();

            try
            {
                if (app.Properties["cerca"].ToString() == "localita")
                {
                    parametri["azione"] = "cercalocalita";
                    parametri["localita"] = app.Properties["localita"].ToString();
                    parametri["provincia"] = app.Properties["provincia"].ToString();

                    localita.Text = app.Properties["localita"].ToString();
                }
                else if (app.Properties["cerca"].ToString() == "posizione")
                {
                    parametri["azione"] = "cercaposizione";

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
                            parametri["latitudine"] = location.Latitude.ToString();
                            parametri["longitudine"] = location.Longitude.ToString();

                            app.Properties["latitudine"] = location.Latitude;
                            app.Properties["longitudine"] = location.Longitude;
                        }
                        else
                            throw new Exception(AppResources.ErrorCoord);
                    }
                    else
                    {
                        parametri["latitudine"] = app.Properties["latitudine"].ToString();
                        parametri["longitudine"] = app.Properties["longitudine"].ToString();
                    }
                }

                var response = InviaRichiesta(parametri);

                if (!response.IsFaulted)
                {
                    var stabilimenti = JsonConvert.DeserializeObject<List<Stabilimento>>(response.Result);
                    _stabilimenti = new ObservableCollection<Stabilimento>(stabilimenti);

                    if (_stabilimenti.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(cerca))
                            listView.ItemsSource = _stabilimenti;
                        else
                            listView.ItemsSource = _stabilimenti.Where(c => c.Nome.StartsWith(cerca));

                        labelvuota.IsVisible = false;
                        listView.IsVisible = true;
                        search.IsVisible = true;
                        localita.IsVisible = true;
                        loader.IsRunning = false;
                        loader.IsVisible = false;
                    }
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
            app.Properties["stabId"] = stabilimento.Id;
            app.Properties["stabNome"] = stabilimento.Nome;
            app.Properties["stabOmbrelloni"] = stabilimento.Ombrelloni;
            app.Properties["stabDisponibili"] = stabilimento.Disponibili;
            app.Properties["stabLatitudine"] = stabilimento.Latitudine;
            app.Properties["stabLongitudine"] = stabilimento.Longitudine;
            app.Properties["stabTelefono"] = stabilimento.Telefono;

            await Navigation.PushAsync(new StabPageUtente());
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopolaListaAsync(e.NewTextValue);
        }

        async private Task<string> InviaRichiesta(Dictionary<string, string> parametri)
        {
            string datiJson = JsonConvert.SerializeObject(parametri);
            var response = _client.PostAsync(Url, new StringContent(datiJson));
            return await response.Result.Content.ReadAsStringAsync();
        }
    }
}