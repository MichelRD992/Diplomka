using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LinearniRovnice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MetodaPage : ContentPage
    {
        public Matice matice { get; set; }
        public MetodaPage()
        {
            InitializeComponent();
        }

        private void bZpet_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void bDale_Clicked(object sender, EventArgs e)
        {
            float[] vysledky = null;
            if (r1.IsChecked) vysledky = matice.Gauss();
            else if (r2.IsChecked) vysledky = matice.Jordan();
            else
            {
                float presnost;
                if (!float.TryParse(ePresnost.Text, out presnost))
                    DisplayAlert("Nastala chyba", "Zadali jste neplatnou přesnost", "OK");
                else if (r3.IsChecked) vysledky = matice.Vypocet(presnost, true);
                else vysledky = matice.Vypocet(presnost, false);
            }
            StackLayout sl = new StackLayout();
            if (vysledky != null)
                Navigation.PushAsync(new VysledekPage(Vysledek(vysledky, sl)));
            else
            {
                sl.Children.Add(new Label() { Text = "Tato metoda pro zadanou matici diverguje, zkuste zvolit jinou." });
                Navigation.PushAsync(new VysledekPage(sl));
            }
        }

        private StackLayout Vysledek(float[] vysledky, StackLayout sl)
        {
            for (int i = 1; i <= vysledky.Length; i++)
                sl.Children.Add(new Label() { Text = $"x{i} = {vysledky[i - 1]}" });
            return sl;
        }

        private void r1_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            sPresnost.IsVisible = false;
        }

        private void r3_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            sPresnost.IsVisible = true;
        }
    }
}