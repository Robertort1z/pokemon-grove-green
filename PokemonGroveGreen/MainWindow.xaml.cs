using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace PokemonGroveGreen
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Author: Roberto Ortiz Perez
        Random r = new Random();
        MediaPlayer buttonSound = new MediaPlayer();
        Trainer trainer;

        public MainWindow()
        {
            InitializeComponent();     

            bgm.Source = new Uri("../../MusicAndSoundEffects/TitleTheme.mp3", UriKind.Relative);
            //Set the MediaEnded event handler to loop the MP3.
            bgm.MediaEnded += LoopTitleTheme;
            //Start playing the MP3.
            bgm.Play();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));

            if (File.Exists("../../trainerData.bin"))
            {
                //Load the trainer from the binary file.
                BinaryFormatter formater = new BinaryFormatter();
                Stream stream = new FileStream("../../trainerData.bin", FileMode.Open);
                trainer = (Trainer)formater.Deserialize(stream);
                stream.Close();
                stream.Dispose();

                //Display info
                continueButton.Content = 
                    "Continue" + "\n" +
                    "Player       " + trainer.GetName() + "\n" +
                    "Time         " + $"{trainer.GetPlayTime().Hours:00}:{trainer.GetPlayTime().Minutes:00}" + "\n" +
                    "Pokémon      " + trainer.GetCapturedPokemon() + "\n" +
                    "Money        " + trainer.GetPokeDollars();
            }
            else
            {
                //We need to create a new game, so we disable the function of the button.
                continueButton.Content = "No save data exists";
                continueButton.Click -= Continue_Click;
            }
        }

        private void LoopTitleTheme(object sender, RoutedEventArgs e)
        {
            //When the music ends, rewind and play again to loop.
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.            
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            frame.NavigationService.Navigate(new MainMenu(r, trainer));
            bgm.Stop();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.            
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            frame.NavigationService.Navigate(new TrainerCreation(r));
            bgm.Stop();
        }
        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (startScene != null && startScene.Parent != null)
                {
                    //Play the clicked button sound effect.            
                    buttonSound.Play();
                    buttonSound.Position = TimeSpan.Zero;

                    ((Grid)startScene.Parent).Children.Remove(startScene);
                    startScene = null; // Optionally, set the reference to null to release memory
                }
            }
        }
    }
}
