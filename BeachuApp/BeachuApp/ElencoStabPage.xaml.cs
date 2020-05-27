using BeachuApp.Resx;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ElencoStabPage : ContentPage
    {
        private readonly HttpClient _client = new HttpClient();
        private ObservableCollection<Stabilimento> _stabilimenti;

        public ElencoStabPage()
        {
            InitializeComponent();
            PopolaListaAsync();
        }

        async private void PopolaListaAsync(string cerca = null)
        {
            try
            {
                string idu = await SecureStorage.GetAsync("beachuid");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

                var response = await _client.GetAsync(Variabili.UrlStab + "gest");

                loader.IsRunning = false;
                loader.IsVisible = false;

                if (response.IsSuccessStatusCode)
                {
                    var resstring = await response.Content.ReadAsStringAsync();
                    var stabilimenti = JsonConvert.DeserializeObject<List<Stabilimento>>(resstring);
                    _stabilimenti = new ObservableCollection<Stabilimento>(stabilimenti);

                    if (_stabilimenti.Count > 0)
                    {
                        if (string.IsNullOrWhiteSpace(cerca))
                            listView.ItemsSource = _stabilimenti;
                        else
                            listView.ItemsSource = _stabilimenti.Where(c => c.nome.StartsWith(cerca));

                        labelaggiorna.IsVisible = true;
                        listView.IsVisible = true;
                        search.IsVisible = true;
                    }
                    else
                        labelvuota.IsVisible = true;
                }
                else
                    labelvuota.IsVisible = true;
            }
            catch
            {
                loader.IsRunning = false;
                loader.IsVisible = false;
                labelvuota.IsVisible = true;
            }
        }

        async private void Aggiungi_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.Properties.Clear();
            await Navigation.PushAsync(new StabPage());
        }

        async private void Modifica_Clicked(object sender, System.EventArgs e)
        {
            var app = Application.Current as App;
            try
            {
                var stabilimento = (sender as MenuItem).CommandParameter as Stabilimento;
                app.Properties["stabId"] = stabilimento._id;
                app.Properties["stabNome"] = stabilimento.nome;
                app.Properties["stabLocalita"] = stabilimento.localita;
                app.Properties["stabProvincia"] = stabilimento.provincia;
                app.Properties["stabOmbrelloni"] = stabilimento.ombrelloni;
                app.Properties["stabDisponibili"] = stabilimento.disponibili;
                app.Properties["stabTel"] = stabilimento.telefono;
                app.Properties["stabMail"] = stabilimento.email;
                app.Properties["stabWeb"] = stabilimento.web;
            }
            catch
            {
                // niente
            }
            finally
            {
                await Navigation.PushAsync(new StabPage());
            }
        }

        async private void Elimina_Clicked(object sender, System.EventArgs e)
        {
            // VERIFICARE SE VENGONO CARICATE TUTTE LE PROPRIETA' DELLA CLASSE O SOLO QUELLE IN XAML
            var stabilimento = (sender as MenuItem).CommandParameter as Stabilimento;
            string idu = await SecureStorage.GetAsync("beachuid");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Funzioni.CodificaId(idu));

            loader.IsRunning = true;
            loader.IsVisible = true;

            var response = await _client.DeleteAsync(Variabili.UrlStab + stabilimento._id.ToString());

            loader.IsRunning = false;
            loader.IsVisible = false;

            if (response.IsSuccessStatusCode)
                _stabilimenti.Remove(stabilimento);
            else
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorOperation, "Ok");
        }

        private void ListView_Refreshing(object sender, System.EventArgs e)
        {
            PopolaListaAsync();
            listView.EndRefresh();
        }

        async private void Stab_Tapped(object sender, System.EventArgs e)
        {
            var stabilimento = (sender as TextCell).CommandParameter as Stabilimento;
            Application.Current.Properties["stabId"] = stabilimento._id;
            await Navigation.PushAsync(new UmbrellaPage());
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopolaListaAsync(e.NewTextValue);
        }
    }
}