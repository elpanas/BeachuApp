using BeachuApp.Resx;
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
            try
            {
                var app = Application.Current as App;

                if (string.IsNullOrWhiteSpace(localita.Text) || string.IsNullOrWhiteSpace(provincia.Text))
                {
                    await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
                }
                else
                {
                    app.Properties["cerca"] = "localita";
                    app.Properties["localita"] = localita.Text;
                    app.Properties["provincia"] = provincia.Text;

                    await Navigation.PushAsync(new StabDispPage());
                }
            }
            catch
            {
                await DisplayAlert(AppResources.ErrorTitle, AppResources.ErrorValues, "Ok");
            }
        }
    }
}