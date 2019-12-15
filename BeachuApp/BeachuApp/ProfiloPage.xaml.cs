using System;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BeachuApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfiloPage : ContentPage
    {
        public ProfiloPage()
        {
            InitializeComponent();
            InizializzaCampi();
        }

        async private void InizializzaCampi()
        {
            try
            {
                nome.Text = await SecureStorage.GetAsync("beachuusername"); ;
                Navigation.RemovePage(new LoginPage());
            }
            catch
            {
                await Navigation.PushAsync(new LoginPage());
            }
        }

        async private void Stabilimenti_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ElencoStabPage());
        }

        async private void Modifica_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ModProfiloPage());
        }
    }
}