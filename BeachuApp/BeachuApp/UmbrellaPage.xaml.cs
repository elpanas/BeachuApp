using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UmbrellaPage : ContentPage
    {
        private const string Url = "https://beachug.herokuapp.com";
        private HttpClient _client = new HttpClient();

        public UmbrellaPage()
        {
            InitializeComponent();
            InitStabilimento();
        }

        private void InitStabilimento()
        {
            try
            {
                Dictionary<string, string> parametri = new Dictionary<string, string>()
                {
                    { "azione", "estraestabilimento" },
                    { "ids", Application.Current.Properties["stabId"].ToString() }
                };

                var response = InviaRichiesta(parametri);

                if (!response.IsFaulted)
                {
                    var stabilimento = JsonConvert.DeserializeObject<Stabilimento>(response.Result);

                    if (stabilimento.Id > 0)
                    {
                        nome.Text = stabilimento.Nome;
                        stepper.Maximum = stabilimento.Ombrelloni;
                        stepper.Value = stabilimento.Disponibili;
                        ombrelloni.Text = "Disponibili: " + stabilimento.Disponibili;
                    }
                }
                else
                    DisplayAlert("Errore", "Errore di connessione", "Ok");
            }
            catch
            {
                DisplayAlert("Errore", "Operazione non riuscita", "Ok");
            }
        }

        private void Ombrelloni_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Dictionary<string, string> parametri = new Dictionary<string, string>()
            {
                { "azione", "aggiornaombrelloni" },
                { "ids", Application.Current.Properties["stabId"].ToString() },
                { "ombrelloni", e.NewValue.ToString() }
            };

            var response = InviaRichiesta(parametri);

            if (!response.IsFaulted)
            {
                if (JsonConvert.DeserializeObject<int>(response.Result) == 1)
                    ombrelloni.Text = "Disponibili: " + stepper.Value;
                else
                    DisplayAlert("Errore", "Modifica non riuscita", "Ok");
            }
            else
                DisplayAlert("Errore", "Errore di connessione", "Ok");
        }

        async private Task<string> InviaRichiesta(Dictionary<string, string> parametri)
        {
            string datiJson = JsonConvert.SerializeObject(parametri);
            var response = _client.PostAsync(Url, new StringContent(datiJson));
            return await response.Result.Content.ReadAsStringAsync();
        }
    }
}