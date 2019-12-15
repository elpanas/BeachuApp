using Newtonsoft.Json;
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
    public partial class ElencoStabPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();
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
                Dictionary<string, string> parametri = new Dictionary<string, string>()
                {
                    { "azione", "listastabilimenti" },
                    { "idu", await SecureStorage.GetAsync("beachuid") }
                };

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

                        labelaggiorna.IsVisible = true;
                        labelvuota.IsVisible = false;
                        listView.IsVisible = true;
                        search.IsVisible = true;
                    }
                }
            }
            catch
            {
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
                app.Properties["stabId"] = stabilimento.Id;
                app.Properties["stabNome"] = stabilimento.Nome;
                app.Properties["stabLocalita"] = stabilimento.Localita;
                app.Properties["stabProvincia"] = stabilimento.Provincia;
                app.Properties["stabOmbrelloni"] = stabilimento.Ombrelloni;
                app.Properties["stabDisponibili"] = stabilimento.Disponibili;
                app.Properties["stabTel"] = stabilimento.Telefono;
                app.Properties["stabMail"] = stabilimento.Email;
                app.Properties["stabWeb"] = stabilimento.Web;
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

        private void Elimina_Clicked(object sender, System.EventArgs e)
        {
            // VERIFICARE SE VENGONO CARICATE TUTTE LE PROPRIETA' DELLA CLASSE O SOLO QUELLE IN XAML
            var stabilimento = (sender as MenuItem).CommandParameter as Stabilimento;

            Dictionary<string, string> parametri = new Dictionary<string, string>()
            {
                { "ids", stabilimento.Id.ToString() },
                { "azione", "eliminastabilimento" }
            };

            var response = InviaRichiesta(parametri);

            if (!response.IsFaulted)
            {
                if (JsonConvert.DeserializeObject<int>(response.Result) == 1)
                    _stabilimenti.Remove(stabilimento);
                else
                    DisplayAlert("Errore", "Eliminazione non riuscita", "Ok");
            }
            else
                DisplayAlert("Errore", "Errore di connessione", "Ok");
        }

        private void ListView_Refreshing(object sender, System.EventArgs e)
        {
            PopolaListaAsync();
            listView.EndRefresh();
        }

        async private void Stab_Tapped(object sender, System.EventArgs e)
        {
            var stabilimento = (sender as TextCell).CommandParameter as Stabilimento;

            Application.Current.Properties["stabId"] = stabilimento.Id;

            await Navigation.PushAsync(new UmbrellaPage());
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