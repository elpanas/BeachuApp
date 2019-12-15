using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BeachuApp
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Login_Clicked(object sender, EventArgs e)
        {
            ControlloLogin();
        }

        async private void ControlloLogin()
        {
            try
            {
                var beachuid = await SecureStorage.GetAsync("beachuid");

                if (!String.IsNullOrEmpty(beachuid))
                    await Navigation.PushAsync(new ProfiloPage());
                else
                    await Navigation.PushAsync(new LoginPage());
            }
            catch
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        async private void Posizione_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["cerca"] = "posizione";
            await Navigation.PushAsync(new StabDispPage());
        }

        async private void Localita_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LocalitaPage());
        }
    }
}
