using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocalitaPage : ContentPage
    {
        public LocalitaPage()
        {
            InitializeComponent();
        }

        async private void Cerca_Clicked(object sender, EventArgs e)
        {
            var app = Application.Current as App;

            try
            {
                if (string.IsNullOrEmpty(localita.Text) || string.IsNullOrEmpty(provincia.Text))
                    await DisplayAlert("Errore", "Alcuni campi non contengono valori accettabili", "Ok");
                else
                {
                    app.Properties["cerca"] = "localita";
                    app.Properties["localita"] = localita.Text;
                    app.Properties["provincia"] = provincia.Text;

                    await Navigation.PushAsync(new StabDispPage());
                }
            }
            catch (Exception)
            {
                await DisplayAlert("Errore", "Alcuni campi non contengono valori accettabili", "Ok");
            }
        }
    }
}