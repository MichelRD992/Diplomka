using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Xamarin.Forms.Grid;

namespace LinearniRovnice
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaticePage : ContentPage
    {
        public Matice matice { get; set; }
        public MaticePage(int velikost)
        {
            InitializeComponent();
            matice = new Matice();
            matice.Vytvor(velikost);

            for (int i = 0; i < velikost; i++)
            {
                for (int j = 0; j < velikost; j++)
                {
                    var entry = new Entry();
                    gMatice.Children.Add(entry, j, i);
                }
                var entry2 = new Entry();
                gMatice.Children.Add(entry2, velikost + 1, i);
            }
        }

        private void bZpet_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void bDale_Clicked(object sender, EventArgs e)
        {
            if(VratHodnoty())
            {
                if(matice.DET(matice.nezname) == 0)
                {
                    StackLayout sl = new StackLayout();
                    sl.Children.Add(new Label() { Text = "Soustava nemá konkrétní řešení" });
                    Navigation.PushAsync(new VysledekPage(sl));
                }
                else
                    Navigation.PushAsync(new MetodaPage() { matice = matice });
            }
        }

        private bool VratHodnoty()
        {
            IGridList<View> entries = gMatice.Children;
            int n = matice.konstanty.Length;
            int x = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!float.TryParse(((Entry)entries[x]).Text, out matice.nezname[i, j]))
                    {
                        DisplayAlert("Nastala chyba", "Zadali jste neplatné číslo", "OK");
                        return false;
                    }
                    x++;
                }
                if (!float.TryParse(((Entry)entries[x]).Text, out matice.konstanty[i]))
                {
                    DisplayAlert("Nastala chyba", "Zadali jste neplatné číslo", "OK");
                    return false;
                }
                x++;
            }
            return true;
        }
    }
}