using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;

namespace PokemonGroveGreen
{
    /// <summary>
    /// Lógica de interacción para MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        //Author: Roberto Ortiz Perez
        //Sound effects.
        MediaPlayer buttonSound = new MediaPlayer();
        MediaPlayer healingSound = new MediaPlayer();
        MediaPlayer savingSound = new MediaPlayer();

        //Stores our pokémon team.
        Pokémon[] playerTeam = new Pokémon[6];

        //Stores the trainer.
        Trainer trainer;
        Random r;

        DateTime startTime;

        public MainMenu(Random r, Trainer trainer)
        {
            InitializeComponent();

            // Record the start time when the program starts
            startTime = DateTime.Now;

            //Load the trainer.
            this.trainer = trainer;
            this.r = r;

            bgm.Source = new Uri("../../MusicAndSoundEffects/MenuTheme.mp3", UriKind.Relative);
            
            //Set the MediaEnded event handler to loop the MP3.
            bgm.MediaEnded += LoopMenuTheme;
            
            //Start playing the MP3.
            bgm.Play();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
            healingSound.Open(new Uri("../../MusicAndSoundEffects/HealingSound.mp3", UriKind.Relative));
            savingSound.Open(new Uri("../../MusicAndSoundEffects/SavingSound.mp3", UriKind.Relative));
        }

        private void LoopMenuTheme(object sender, RoutedEventArgs e)
        {
            //When the music ends, rewind and play again to loop.
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
        }

        private void Battle_Click(object sender, RoutedEventArgs e)
        {
            //Check we have at least one pokemon alive.
            Pokémon pokemon = trainer.GetPokemonInTeam(0);
            if (pokemon != null && pokemon.GetCurrentHP() >= 1)
            {
                //Play the clicked button sound effect.
                buttonSound.Play();
                buttonSound.Position = TimeSpan.Zero;

                //Enter a new battle
                this.NavigationService.Navigate(new Battle(r, trainer));
                bgm.Stop();
            }
            else
            {
                for (int i = 1; i < 6; i += 1)
                {
                    pokemon = trainer.GetPokemonInTeam(i);
                    if (pokemon != null)
                    {
                        if (trainer.GetPokemonInTeam(i).GetCurrentHP() >= 1)
                        {
                            trainer.SwapPokemonTeamPositions(0, i);
                            //Enter a new battle
                            this.NavigationService.Navigate(new Battle(r, trainer));
                            bgm.Stop();
                            break;
                        }
                    }
                }
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Ask for user confirmation.
            MessageBoxResult exit = MessageBox.Show("Are you sure you want to exit the game? " +
                "All unsaved data will be lost.", "Confirmation", MessageBoxButton.YesNo);
            
            //Exit the program.
            if (exit == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private async void PokémonCenter_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Position = TimeSpan.Zero;
            buttonSound.Play();

            //Stop the BGM from playing.
            bgm.Stop();

            //Change the screen to black.
            blackScreen.Opacity = 1;

            //Play the healing sound.
            healingSound.Play();
            healingSound.Position = TimeSpan.Zero; //Restores the sound to its beginning.

            //HERE WE HEAL THE POKÉMON.
            //Pokémon team.
            for (int i = 0; i < 6; i += 1)
            {
                //Once we do not have a pokémon we have finished.
                if (trainer.GetPokemonInTeam(i) == null)
                    break;
                trainer.GetPokemonInTeam(i).Heal();
            }
            
            //Pokémon Vault
            for (int vault = 0; vault < 3; vault += 1)
            {
                for (int index = 0; index < 30; index += 1)
                {
                    if (trainer.GetPokemonInVault(vault, index) != null)
                        trainer.GetPokemonInVault(vault, index).Heal();
                }
            }

            //Waits until the sound ends.
            await Task.Delay(TimeSpan.FromSeconds(healingSound.NaturalDuration.TimeSpan.TotalSeconds));

            //We restore everything back to normal.
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
            blackScreen.Opacity = 0;
        }

        private void Data_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //We need to update the play time before displaying the data.
            trainer.AddPlayTime(DateTime.Now - startTime);

            //Update startTime for the next time we save.
            startTime = DateTime.Now;

            //Display the trainer's data.
            this.NavigationService.Navigate(new TrainerCard(trainer));
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Play the saving button sound effect.
            savingSound.Play();
            savingSound.Position = TimeSpan.Zero;

            //We need to update the play time before saving the data.
            trainer.AddPlayTime(DateTime.Now - startTime);

            //Update startTime for the next time we save.
            startTime = DateTime.Now;

            //Save data on a binary file.
            BinaryFormatter formater = new BinaryFormatter();
            Stream stream = new FileStream("../../trainerData.bin", FileMode.Create);
            formater.Serialize(stream, trainer);
            stream.Close();
            stream.Dispose();
        }

        private void Bag_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the bag
            this.NavigationService.Navigate(new Bag(trainer, false, false));
        }

        private void Shop_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Enter to the shop.
            this.NavigationService.Navigate(new ShopSelection(trainer));
            bgm.Stop();
        }

        private void SaveToText_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Play the saving button sound effect.
            savingSound.Play();
            savingSound.Position = TimeSpan.Zero;

            //We need to update the play time before saving the data.
            trainer.AddPlayTime(DateTime.Now - startTime);

            StreamWriter sw = new StreamWriter("../../trainerTextData.txt");
            sw.WriteLine(trainer.ToString());
            sw.Close();
        }

        private void Vault_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Display the trainer's data.
            this.NavigationService.Navigate(new PokemonVault(trainer));
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change cursor to hand when mouse enters the cell
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            // Reset cursor when mouse leaves the cell
            Mouse.OverrideCursor = null;
        }
    }
}
