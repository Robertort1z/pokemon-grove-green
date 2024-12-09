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
    /// Lógica de interacción para TrainerCreation.xaml
    /// </summary>
    public partial class TrainerCreation : Page
    {
        //Author: Roberto Ortiz Perez
        Random r;

        //Sound effects.
        MediaPlayer buttonSound = new MediaPlayer();

        public TrainerCreation(Random r)
        {
            InitializeComponent();
            this.r = r;

            bgm.Source = new Uri("../../MusicAndSoundEffects/TrainerCreationTheme.mp3", UriKind.Relative);

            //Set the MediaEnded event handler to loop the MP3.
            bgm.MediaEnded += LoopCreationTheme;

            //Start playing the MP3.
            bgm.Play();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
        }

        private void LoopCreationTheme(object sender, RoutedEventArgs e)
        {
            //When the music ends, rewind and play again to loop.
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Check the user has filled in all fields. We do nothing if he has not.
            if (nameBox.Text == "" || genderBox.SelectedIndex == 0 || regionBox.SelectedIndex == 0)
                return;

            //Check which gender did the user select.
            Gender gender;
            switch (genderBox.SelectedIndex)
            {
                case 1:
                    {
                        gender = Gender.MALE;
                        break;
                    }
                case 2:
                    {
                        gender = Gender.FEMALE;
                        break;
                    }
                case 3:
                    {
                        gender = Gender.GENDERLESS;
                        break;
                    }
                default:
                    {
                        //Not happening. But compiler.
                        gender = Gender.GENDERLESS;
                        break;
                    }
            }

            //Check which region did the user select.
            int regionId;
            switch (regionBox.SelectedIndex)
            {
                case 1:
                    {
                        //Europe
                        regionId = 0;
                        break;
                    }
                case 2:
                    {
                        //NA
                        regionId = 1;
                        break;
                    }
                case 3:
                    {
                        //Asia-Australia
                        regionId = 2;
                        break;
                    }
                case 4:
                    {
                        //Africa
                        regionId = 3;
                        break;
                    }
                case 5:
                    {
                        //SA
                        regionId = 4;
                        break;
                    }
                default:
                    {
                        //Not happening. But compiler.
                        regionId = 0;
                        break;
                    }
            }

            //We create the new trainer.
            Trainer trainer = new Trainer(r, nameBox.Text, gender, regionId);
            
            //Go to the main menu.
            this.NavigationService.Navigate(new MainMenu(r, trainer));
            bgm.Stop();
        }

        private void Gender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Check and replace the trainer image.
            string newSource;
            switch (genderBox.SelectedIndex)
            {
                case 1:
                    {
                        newSource = "/Images/maleTrainer.png";
                        break;
                    }
                case 2:
                    {
                        newSource = "/Images/femaleTrainer.png";
                        break;
                    }
                case 3:
                    {
                        newSource = "/Images/otherTrainer.png";
                        break;
                    }
                default:
                    {
                        newSource = string.Empty;
                        break;
                    }
            }

            trainerImage.Source = new BitmapImage(new Uri(newSource, UriKind.Relative));
        }

        private void Region_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Check and replace the region image.
            string newSource;
            switch (regionBox.SelectedIndex)
            {
                case 1:
                    {
                        newSource = "/Images/europe.png";
                        break;
                    }
                case 2:
                    {
                        newSource = "/Images/northAmerica.png";
                        break;
                    }
                case 3:
                    {
                        newSource = "/Images/asia.png";
                        break;
                    }
                case 4:
                    {
                        newSource = "/Images/africa.png";
                        break;
                    }
                case 5:
                    {
                        newSource = "/Images/southAmerica.png";
                        break;
                    }
                default:
                    {
                        newSource = string.Empty;
                        break;
                    }
            }
            regionImage.Source = new BitmapImage(new Uri(newSource, UriKind.Relative));
        }
    }
}
