using BeachuApp.Resx;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StabPageUtente : ContentPage
    {
        public StabPageUtente()
        {
            InitializeComponent();
            PopolaPagina();
        }

        private void PopolaPagina()
        {
            var app = Application.Current as App;

            try
            {
                if (app.Properties["stabTelefono"].ToString() != "0") phone.IsEnabled = true;
            }
            catch
            {
                // niente
            }

            nome.Text = app.Properties["stabNome"].ToString();
            disponibili.Text = "Ombrelloni: " + app.Properties["stabDisponibili"].ToString();
        }

        private void Phone_Clicked(object sender, EventArgs e)
        {
            string numero = Application.Current.Properties["stabTelefono"].ToString();
            PhoneCall(numero);
        }

        private void PhoneCall(string number)
        {
            try
            {
                PhoneDialer.Open(number);
            }
            catch (ArgumentNullException anEx)
            {
                DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorNr + anEx, "Ok");
            }
            catch (FeatureNotSupportedException ex)
            {
                DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorPh + ex, "Ok");
            }
            catch (Exception ex)
            {
                DisplayAlert(AppResources.ErrorTitle, "Error: " + ex, "Ok");
            }
        }

        async private void Mappa_Clicked(object sender, EventArgs e)
        {
            var app = Application.Current as App;

            double latitudine = Convert.ToDouble(app.Properties["stabLatitudine"]);
            double longitudine = Convert.ToDouble(app.Properties["stabLongitudine"]);

            var location = new Location(latitudine, longitudine);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            await Map.OpenAsync(location, options);
        }
    }
}