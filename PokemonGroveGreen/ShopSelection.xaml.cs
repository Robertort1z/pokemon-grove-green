using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokemonGroveGreen
{
    /// <summary>
    /// Lógica de interacción para ShopSelection.xaml
    /// </summary>
    public partial class ShopSelection : Page
    {
        //Author: Roberto Ortiz Perez
        MediaPlayer buttonSound = new MediaPlayer();
        Trainer trainer;

        public ShopSelection(Trainer trainer)
        {
            InitializeComponent();
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
            this.trainer = trainer;
        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Enter a new battle
            this.NavigationService.Navigate(new ShopBuy(trainer));
        }

        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Enter a new battle
            this.NavigationService.Navigate(new Bag(trainer, true, false));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Enter a new battle
            this.NavigationService.GoBack();
        }
    }
}
