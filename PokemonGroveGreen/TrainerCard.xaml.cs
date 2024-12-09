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
    /// Lógica de interacción para TrainerCard.xaml
    /// </summary>
    public partial class TrainerCard : Page
    {
        //Author: Roberto Ortiz Perez
        Trainer trainer;
        MediaPlayer buttonSound = new MediaPlayer();

        public TrainerCard(Trainer trainer)
        {
            InitializeComponent();
            this.trainer = trainer;

            UpdateCard();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));

            
        }

        private void UpdateCard()
        {
            //Update the trainer's image.
            trainerImage.Source = new BitmapImage(new Uri(trainer.GetSprite(), UriKind.Relative));

            //Update the trainer's name.
            nameBlock.Text = trainer.GetName();

            //Update the trainer's gender.
            switch (trainer.GetGender())
            {
                case Gender.MALE:
                    {
                        genderBlock.Text = "Male";
                        break;
                    }
                case Gender.FEMALE:
                    {
                        genderBlock.Text = "Female";
                        break;
                    }
                case Gender.GENDERLESS:
                    {
                        genderBlock.Text = "Other";
                        break;
                    }
            }


            //Update the trainer's ID.
            idBlock.Text = trainer.GetId();

            //Update and display the trainer's play time.
            timePlayedBlock.Text = $"{trainer.GetPlayTime().Hours:00}:{trainer.GetPlayTime().Minutes:00}";

            //Update the trainer's money.
            moneyBlock.Text = "¥" + trainer.GetPokeDollars().ToString();

            //Update the trainer's points.
            pointsBlock.Text = trainer.GetBattlePoints().ToString() + " BP / "
                + trainer.GetPokeMiles().ToString() + " PM";

            int counter = 0;
            // Update the trainer's pokemon's image.
            foreach (UIElement child in pokemonTeam.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains a Grid
                    if (border.Child is Grid grid)
                    {
                        // Get the Image and ProgressBar inside the Grid
                        Image image = null;
                        ProgressBar progressBar = null;

                        foreach (UIElement gridChild in grid.Children)
                        {
                            if (gridChild is Image)
                            {
                                image = gridChild as Image;
                            }
                            else if (gridChild is ProgressBar)
                            {
                                progressBar = gridChild as ProgressBar;
                            }
                        }

                        // Check if both Image and ProgressBar are found
                        if (image != null && progressBar != null)
                        {
                            // Update the Image source
                            string sprite = trainer.GetPokemonTeamFrontSprite(counter);
                            if (!string.IsNullOrEmpty(sprite))
                            {
                                Pokémon aux = trainer.GetPokemonInTeam(counter);

                                image.Source = new BitmapImage(new Uri(sprite, UriKind.Relative));
                                
                                progressBar.Visibility = Visibility.Visible; // Show progress bar   
                                progressBar.Maximum = aux.GetSpecie().GetHP();
                                progressBar.Value = aux.GetCurrentHP();
                                UpdateHealthBar(progressBar);
                            }
                            else
                            {
                                image.Source = null; // Clear image source
                                progressBar.Visibility = Visibility.Collapsed; // Hide progress bar
                            }                           
                        }
                    }
                }
                counter += 1;
            }
        }

        private void UpdateHealthBar(ProgressBar progressBar)
        {
            int percentage = (int)Math.Round(progressBar.Value * 100 / progressBar.Maximum);
            if (percentage <= 20)
                progressBar.Foreground = new SolidColorBrush(Colors.Red);
            else if (percentage <= 50)
                progressBar.Foreground = new SolidColorBrush(Colors.Yellow);
        }

        //To select cells (to swap and see details).
        private Border selectedCell;

        private void Cell_MouseDown(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            // Get the clicked cell
            Border clickedCell = sender as Border;

            if (selectedCell == null)
            {
                // First cell selection
                selectedCell = clickedCell;
                selectedCell.BorderBrush = Brushes.Red;
                selectedCell.BorderThickness = new Thickness(5);
            }
            else
            {
                // Second cell selection
                SwapContent(selectedCell, clickedCell);
                selectedCell.BorderThickness = new Thickness(0); //Reset border
                selectedCell = null; // Reset selection
            }
        }

        private void SwapContent(Border cell1, Border cell2)
        {
            // Get the grid controls inside the selected cells
            Grid grid1 = cell1.Child as Grid;
            Grid grid2 = cell2.Child as Grid;

            // Ensure both grids exist
            if (grid1 != null && grid2 != null)
            {
                // Get the image and progress bar controls inside the grids
                Image image1 = grid1.Children.OfType<Image>().FirstOrDefault();
                ProgressBar progressBar1 = grid1.Children.OfType<ProgressBar>().FirstOrDefault();
                Image image2 = grid2.Children.OfType<Image>().FirstOrDefault();
                ProgressBar progressBar2 = grid2.Children.OfType<ProgressBar>().FirstOrDefault();

                // Ensure both image and progress bar controls exist
                if (image1 != null && progressBar1 != null && image2 != null && progressBar2 != null)
                {
                    // Swap the image sources
                    ImageSource source1 = image1.Source;
                    ImageSource source2 = image2.Source;

                    image1.Source = source2;
                    image2.Source = source1;

                    // Swap the progress bar properties
                    double maxValue1 = progressBar1.Maximum;
                    double value1 = progressBar1.Value;
                    Brush foreground1 = progressBar1.Foreground;

                    progressBar1.Maximum = progressBar2.Maximum;
                    progressBar1.Value = progressBar2.Value;
                    progressBar1.Foreground = progressBar2.Foreground;

                    progressBar2.Maximum = maxValue1;
                    progressBar2.Value = value1;
                    progressBar2.Foreground = foreground1;

                    // After swapping, update the actual position on the team
                    int firstPokemon = VariableToInt(cell1.Name);
                    int secondPokemon = VariableToInt(cell2.Name);

                    trainer.SwapPokemonTeamPositions(firstPokemon, secondPokemon);
                }
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change cursor to hand when mouse enters the cell
            Mouse.OverrideCursor = Cursors.Hand;

            //Display a slight border. Only if the border is not the selected one.
            Border hoveredCell = sender as Border;
            if (selectedCell != hoveredCell)
            {
                hoveredCell.BorderBrush = Brushes.Blue;
                hoveredCell.BorderThickness = new Thickness(3);
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            // Reset cursor when mouse leaves the cell
            Mouse.OverrideCursor = null;

            //Remove the border. Only if the border is not selected.
            Border hoveredCell = sender as Border;
            if (selectedCell != hoveredCell)
                hoveredCell.BorderThickness = new Thickness(0);
        }

        private int VariableToInt(string name)
        {
            switch (name)
            {
                case "zero":
                    return 0;
                case "first":
                    return 1;
                case "second":
                    return 2;
                case "third":
                    return 3;
                case "fourth":
                    return 4;
                case "fifth":
                    return 5;
                default:
                    throw new ArgumentException("Invalid variable", nameof(name));
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            //Displays details of the selected pokémon.
            if (selectedCell != null)
            {
                //Play the clicked button sound effect.
                buttonSound.Play();
                buttonSound.Position = TimeSpan.Zero;

                int pokemonIndex = VariableToInt(selectedCell.Name);

                //Show details.
                this.NavigationService.Navigate(new PokemonDetails(trainer.GetPokemonInTeam(pokemonIndex)));
                selectedCell.BorderThickness = new Thickness(0);
                selectedCell = null; // Reset selection
            }
        }
    }
}
