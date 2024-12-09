using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PokemonGroveGreen
{
    /// <summary>
    /// Lógica de interacción para PokemonVault.xaml
    /// </summary>
    public partial class PokemonVault : Page
    {
        //Author: Roberto Ortiz Perez
        Trainer trainer;
        MediaPlayer buttonSound = new MediaPlayer();

        public PokemonVault(Trainer trainer)
        {
            InitializeComponent();
            this.trainer = trainer;

            UpdateVault();
            UpdateTeamImages();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
        }

        private void UpdateVault()
        {
            //We always enter to the first vault.
            vaultBlock.Text = "Vault 1";

            int vault = GetVaultNumber();
            UpdateVaultImages(vault, false);        
        }

        private void UpdateVaultImages(int vault, bool keepVoid)
        {
            int counter = 0;
            //Update the vault's pokemons images..
            foreach (UIElement child in pokemonVault.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image
                    if (border.Child is Image image)
                    {
                        image.Source = new BitmapImage(new Uri(trainer.GetPokemonVaultFrontSprite(vault, counter, keepVoid), UriKind.Relative));
                        counter += 1;
                    }
                }
            }
        }

        private void UpdateTeamImages()
        {
            int counter = 0;
            //Update the trainer's pokemon's image.
            foreach (UIElement child in pokemonTeam.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image
                    if (border.Child is Image image)
                    {
                        image.Source = new BitmapImage(new Uri(trainer.GetPokemonTeamFrontSprite(counter), UriKind.Relative));
                        counter += 1;
                    }
                }
            }
        }

        //To select cells (to swap and see details).
        Image selectedImage;
        Border selectedBorder;
        int firstVault;
        ImageSource firstSource;

        private void Cell_MouseDown(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            // Get the clicked image
            Image clickedImage = sender as Image;

            if (selectedImage == null)
            {
                // First cell selection
                selectedImage = clickedImage;

                //Update border.
                selectedBorder = GetParentBorderOfImage(clickedImage);
                selectedBorder.BorderThickness = new Thickness(1);
                selectedBorder.BorderBrush = Brushes.Red;

                firstVault = GetVaultNumber();
                firstSource = selectedImage.Source;

                //We add void to the rest of the vault so we can change to an empty space.
                AddVoid();
            }
            else
            {
                // Second cell selection
                SwapContent(selectedImage, clickedImage);
                ResetSelection();
                RemoveVoid();
            }
        }

        private void AddVoid()
        {
            //Add void to the empty elements of the grid so we can swap pokémon.
            foreach (UIElement child in pokemonVault.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image.
                    if (border.Child is Image image)
                    {
                        // Check if the Image source is empty
                        if (image.Source == null || image.Source.ToString() == string.Empty)
                        {
                            image.Source = new BitmapImage(new Uri("/Images/void.png", UriKind.Relative));                            
                        }
                    }
                }
            }

            //Same thing but for the team
            foreach (UIElement child in pokemonTeam.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image.
                    if (border.Child is Image image)
                    {
                        // Check if the Image source is empty
                        if (image.Source == null || image.Source.ToString() == string.Empty)
                        {
                            image.Source = new BitmapImage(new Uri("/Images/void.png", UriKind.Relative));
                        }
                    }
                }
            }
        }

        private void RemoveVoid()
        {
            //Remove void to the empty elements of the grid so we can swap pokémon.
            foreach (UIElement child in pokemonVault.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image.
                    if (border.Child is Image image)
                    {
                        // Check if the Image source is the void.
                        if (image.Source != null && image.Source.ToString() == "pack://application:,,,/Images/void.png")
                        {
                            image.Source = null;
                        }
                    }
                }
            }

            //Same thing but for the team
            foreach (UIElement child in pokemonTeam.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image.
                    if (border.Child is Image image)
                    {
                        // Check if the Image source is the void.
                        if (image.Source != null && image.Source.ToString() == "pack://application:,,,/Images/void.png")
                        {
                            image.Source = null;
                        }
                    }
                }
            }
        }

        private void SwapContent(Image image1, Image image2)
        {
            //We swap the displayed image and the position of the pokémon.

            //With this we know if we are changing between vaults.
            int secondVault = GetVaultNumber();

            string firstGrid = GetGridNameFromImage(image1);
            string secondGrid = GetGridNameFromImage(image2);
            
            //We need to know if we are changing between two vaults or with the team.
            if (firstGrid != secondGrid) //Change between vault and team. Both images change.
            {
                //We only allow the change if there is at least one pokemon on the team.
                if (trainer.GetPokemonInTeam(1) == null) //There is only one pokémon.
                {
                    //We allow the change ONLY if we change with other pokémon.
                    if (firstGrid == "pokemonVault") //It is the first one comes from the vault
                    {
                        if (image1.Source.ToString() == "pack://application:,,,/Images/void.png")
                            return;
                    }
                    else //It is the second one comes from the vault
                    {
                        if (image2.Source.ToString() == "pack://application:,,,/Images/void.png")
                            return;
                    }
                }

                int vaultIndex;
                int teamIndex;
                //We need to know which pokemon comes from the team and which from the vault.
                if (firstGrid == "pokemonVault")
                {
                    //The first one comes from the vault.
                    vaultIndex = GetPokemonVaultIndex(image1);
                    teamIndex = GetPokemonTeamIndex(image2);
                    
                    //We need the program to act different if we are changing in the same vault or not.
                    if (firstVault == secondVault)
                    {
                        // Swap the image sources
                        ImageSource source2 = image2.Source;

                        image1.Source = source2;
                        image2.Source = firstSource;
                    }
                    else
                    {
                        //If we have different vaults we replace the second cell with the image of the first one.
                        image2.Source = firstSource;
                    }
                    trainer.SwapPokemonBetweenVaultAndTeam(firstVault, vaultIndex, teamIndex);
                }
                else
                {
                    //The first one comes from the team.
                    vaultIndex = GetPokemonVaultIndex(image2);
                    teamIndex = GetPokemonTeamIndex(image1);

                    //We need the program to act different if we are changing in the same vault or not.
                    if (firstVault == secondVault)
                    {
                        // Swap the image sources
                        ImageSource source2 = image2.Source;

                        image1.Source = source2;
                        image2.Source = firstSource;
                    }
                    else
                    {
                        //If we have different vaults we replace the second cell with the image of the first one.
                        image2.Source = firstSource;
                    }
                    trainer.SwapPokemonBetweenVaultAndTeam(secondVault, vaultIndex, teamIndex);
                }

                //Adjust the team so we do not have empty spaces.
                RelocatePokemonTeam(trainer.GetPokemonTeam());
                UpdateTeamImages();
            }
            else if (firstGrid == "pokemonVault") //Change inside vaults.
            {
                //We need the program to act different if we are changing in the same vault or not.
                if (firstVault == secondVault)
                {
                    // Swap the image sources
                    ImageSource source2 = image2.Source;

                    image1.Source = source2;
                    image2.Source = firstSource;
                }
                else
                {
                    //If we have different vaults we replace the second cell with the image of the first one.
                    image2.Source = firstSource;
                }

                //Change them in the trainer pokemon vault.
                int firstPokemon = GetPokemonVaultIndex(image1);
                int secondPokemon = GetPokemonVaultIndex(image2);

                trainer.SwapPokemonVaultPositions(firstVault, secondVault, firstPokemon, secondPokemon);
            }
            else //Change inside team.
            {
                // Swap the image sources
                ImageSource source2 = image2.Source;

                image1.Source = source2;
                image2.Source = firstSource;

                //Change them in the trainer pokemon team.
                int firstPokemon = GetPokemonTeamIndex(image1);
                int secondPokemon = GetPokemonTeamIndex(image2);

                trainer.SwapPokemonTeamPositions(firstPokemon, secondPokemon);

                //Adjust the team so we do not have empty spaces.
                RelocatePokemonTeam(trainer.GetPokemonTeam());
                UpdateTeamImages();
            }
        }

        private void RelocatePokemonTeam(Pokémon[] team)
        {
            for (int i = 0; i < team.Length; i += 1)
            {
                if (team[i] == null)
                {
                    //Push non-null Pokémon forward
                    for (int j = i + 1; j < team.Length; j += 1)
                    {
                        if (team[j] != null)
                        {
                            team[i] = team[j];
                            team[j] = null;
                            break;
                        }
                    }

                    //If no non-null Pokémon found after i, break
                    if (team[i] == null)
                    {
                        break;
                    }
                }
            }
        }

        private string GetGridNameFromImage(Image image)
        {
            //Used to know if a pokémon comes from a vault or from the team.            ;
            
            //We get the parent of the image, and then the parent of the border (always a grid).
            DependencyObject parent = VisualTreeHelper.GetParent(VisualTreeHelper.GetParent(image));
            Grid grid = (Grid)parent;

            return grid.Name;
        }

        private string GetGridNameFromBorder(Border border)
        {
            //Used to know if a pokémon comes from a vault or from the team.            ;
            
            //We get the parent of the border, (always a grid).
            DependencyObject parent = VisualTreeHelper.GetParent(border);
            Grid grid = (Grid)parent;

            return grid.Name;
        }

        private Border GetParentBorderOfImage(Image image)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(image);

            return parent as Border;
        }

        private int GetPokemonVaultIndex(Image image)
        {
            //We get the parent of the image.
            Border cell = image.Parent as Border;

            switch (Grid.GetRow(cell))
            {
                case 0:
                    {
                        return Grid.GetColumn(cell);
                    }
                case 1:
                    {
                        return Grid.GetColumn(cell) + 6;
                    }
                case 2:
                    {
                        return Grid.GetColumn(cell) + 12;
                    }
                case 3:
                    {
                        return Grid.GetColumn(cell) + 18;
                    }
                case 4:
                    {
                        return Grid.GetColumn(cell) + 24;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid variable", nameof(image));
                    }
            }
        }

        private int GetPokemonTeamIndex(Image image)
        {
            //We get the parent of the image.
            Border cell = image.Parent as Border;

            switch (Grid.GetRow(cell))
            {
                case 0:
                    {
                        return Grid.GetColumn(cell);
                    }
                case 1:
                    {
                        return Grid.GetColumn(cell) + 2;
                    }
                case 2:
                    {
                        return Grid.GetColumn(cell) + 4;
                    }
                default:
                    { 
                        throw new ArgumentException("Invalid variable", nameof(image));
                    }
            }
        }

        private int GetVaultNumber()
        {
            //Get the box we are currently in.
            int number = 0; // Initialize the variable to store the number

            // Split the string by space
            string[] parts = vaultBlock.Text.Split(' ');

            // Check if there are at least two parts
            if (parts.Length >= 2)
            {
                // Try to parse the second part as an integer
                int.TryParse(parts[1], out number);
            }

            return number - 1;
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change cursor to hand when mouse enters the cell
            Mouse.OverrideCursor = Cursors.Hand;

            //Display a slight border. Only if the border is not the selected one.
            Border hoveredCell = sender as Border;
            Image image = hoveredCell.Child as Image;

            if (selectedBorder != hoveredCell && image != null && image.Source != null)
            {
                hoveredCell.BorderBrush = Brushes.Blue;
                hoveredCell.BorderThickness = new Thickness(0.5);
            }
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            // Reset cursor when mouse leaves the cell
            Mouse.OverrideCursor = null;

            //Remove the border. Only if the border is not selected.
            Border hoveredCell = sender as Border;
            if (selectedBorder != hoveredCell)
                hoveredCell.BorderThickness = new Thickness(0.01);
        }

        private void Back_Click(object sender, MouseButtonEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
        }

        private void ChangeVault_Click(object sender, MouseButtonEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //First we need to know in which vault we are.
            int currentVault = GetVaultNumber();

            //Are we moving back or forward?
            Border clickedCell = sender as Border;
            int newVault;
            if (clickedCell.Name == "vaultBack") //We move back.
            {
                if (currentVault == 0) //If we are on the first, we go to the last.
                    newVault = 2;
                else 
                    newVault = currentVault - 1;
            }
            else //We move forward.
            {
                if (currentVault == 2) //If we are on the last, we go to the first.
                    newVault = 0;
                else
                    newVault = currentVault + 1;
            }
           
            if (selectedBorder != null) //There is border.
            {
                //We need to show or hide the selectedBorder. Not if it is in the team.
                string grid = GetGridNameFromBorder(selectedBorder);

                if (grid != "pokemonTeam") //We skip this if the border is in the team.
                {
                    //Are we showing or hiding.
                    if (firstVault == newVault) //Showing the border.
                    {
                        selectedBorder.BorderBrush = Brushes.Red;
                        selectedBorder.BorderThickness = new Thickness(1);
                    }
                    else //Hiding the border.
                    {
                        selectedBorder.BorderBrush = Brushes.Black;
                        selectedBorder.BorderThickness = new Thickness(0.01);
                    }
                }
            }

            //Update the visual aspects of the vault.
            vaultBlock.Text = "Vault " + (newVault + 1).ToString();

            //If selectedImage is not null we are moving pokémon, so we need to keep the void.
            if (selectedImage != null)
                UpdateVaultImages(newVault, true);
            else
                UpdateVaultImages(newVault, false);
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            //Displays details of the selected pokémon.
            if (selectedImage != null)
            {
                //Play the clicked button sound effect.
                buttonSound.Play();
                buttonSound.Position = TimeSpan.Zero;

                //Vault or team pokemon
                if (GetGridNameFromImage(selectedImage) == "pokemonVault") //Vault
                {
                    int pokemonIndex = GetPokemonVaultIndex(selectedImage);
                    this.NavigationService.Navigate(new PokemonDetails(trainer.GetPokemonInVault(firstVault, pokemonIndex)));
                }
                else //Team
                {
                    int pokemonIndex = GetPokemonTeamIndex(selectedImage);
                    this.NavigationService.Navigate(new PokemonDetails(trainer.GetPokemonInTeam(pokemonIndex)));
                }
            }
            ResetSelection();
        }

        private void Release_Click(object sender, RoutedEventArgs e)
        {
            //We select a pokémon and have at least two pokémon.
            if (selectedImage != null && trainer.GetPokemonInTeam(1) != null)
            {
                //Play the clicked button sound effect.
                buttonSound.Play();
                buttonSound.Position = TimeSpan.Zero;

                //Ask for user confirmation.
                MessageBoxResult confirmation = MessageBox.Show("Are you sure you want to release this pokémon? " +
                    "It will be gone forever. Forever is a long time!", "Confirmation", MessageBoxButton.YesNo);

                //Release the pokemon.
                if (confirmation == MessageBoxResult.Yes)
                {
                    //Vault or team pokemon
                    if (GetGridNameFromImage(selectedImage) == "pokemonVault") //Vault
                    {
                        //We only need to update the image if we are on the selected vault.
                        int currentVault = GetVaultNumber();
                        if (firstVault == currentVault)
                        {
                            selectedImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));
                        }

                        //Release the pokémon.
                        int pokemonIndex = GetPokemonVaultIndex(selectedImage);
                        trainer.ReleasePokemom(firstVault, pokemonIndex);
                    }
                    else //Team
                    {
                        int pokemonIndex = GetPokemonTeamIndex(selectedImage);
                        trainer.ReleasePokemom(pokemonIndex);

                        //Adjust the team so we do not have empty spaces.
                        RelocatePokemonTeam(trainer.GetPokemonTeam());
                        UpdateTeamImages();
                    }

                    ResetSelection();
                }
                else
                {
                    ResetSelection();
                }
            }
            else
            {
                ResetSelection();
            }
        }

        private void ResetSelection()
        {
            selectedImage = null; //Reset selection.
            selectedBorder.BorderBrush = Brushes.Black;
            selectedBorder.BorderThickness = new Thickness(0.01);
            selectedBorder = null;
        }
    }
}
