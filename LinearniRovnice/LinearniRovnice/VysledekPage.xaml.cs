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
    public partial class VysledekPage : ContentPage
    {
        public VysledekPage(StackLayout sl)
        {
            InitializeComponent();
            sVysledky.Children.Add(sl);
        }

        private void bZpet_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }
    }
}