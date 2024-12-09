using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Lógica de interacción para Battle.xaml
    /// </summary>
    public partial class Battle : Page
    {
        //Author: Roberto Ortiz Perez
        MediaPlayer buttonSound = new MediaPlayer();
        MediaPlayer runAwaySound = new MediaPlayer();

        Trainer trainer;
        Random r;

        Pokémon ally;
        Pokémon enemy;

        int flightAttempts = 0;

        public Battle(Random r, Trainer trainer)
        {
            InitializeComponent();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
            runAwaySound.Open(new Uri("../../MusicAndSoundEffects/RunAwaySound.mp3", UriKind.Relative));

            bgm.Source = new Uri("../../MusicAndSoundEffects/BattleTheme.mp3", UriKind.Relative);

            //Set the MediaEnded event handler to loop the MP3.
            bgm.MediaEnded += LoopBattleTheme;

            //Start playing the MP3.
            bgm.Play();

            this.trainer = trainer;
            this.r = r;

            ally = trainer.GetPokemonInTeam(0);
            UpdateAllyUI();
            UpdatePokemonPanel();

            GenerateNewWildPokémon();
        }

        private void LoopBattleTheme(object sender, RoutedEventArgs e)
        {
            //When the music ends, rewind and play again to loop.
            bgm.Position = TimeSpan.Zero;
            bgm.Play();
        }

        private void UpdatePokemonPanel()
        {
            int counter = 0;
            //Update the trainer's pokemon's image.
            foreach (UIElement child in pokemonPanel.Children)
            {
                // Check if the child element is a Border
                if (child is Border border)
                {
                    // Check if the Border contains an Image
                    if (border.Child is Image image)
                    {
                        //Update image
                        image.Source = new BitmapImage(new Uri(trainer.GetPokemonTeamFrontSprite(counter), UriKind.Relative));

                        //Update background color.
                        if (trainer.GetPokemonInTeam(counter) == null) //No pokemon
                        {
                            border.Background = null;
                        }
                        else if (trainer.GetPokemonInTeam(counter).GetCurrentHP() > 0) //Defeated pokemon
                        {
                            SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("LightGreen"));
                            border.Background = brush;
                        }
                        else //Alive pokemon
                        {
                            SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom("DarkOrange"));
                            border.Background = brush;
                        }
                        counter += 1;
                        if (counter == 6) //To avoid the back button.
                            return;
                    }
                }
            }
        }

        private void UpdateAllyUI()
        {
            //We change the buttons to display the moves our pokémon has available.

            //Change the text of the button.
            firstMoveText.Text = " " + ally.GetMoveName(0) + " ";
            secondMoveText.Text = " " + ally.GetMoveName(1) + " ";
            thirdMoveText.Text = " " + ally.GetMoveName(2) + " ";
            fourthMoveText.Text = " " + ally.GetMoveName(3) + " ";

            //Change the PP of each move. Only if there is a move in that slot.
            if (ally.GetMove(0).GetName() != string.Empty)
                firstMovePP.Text = ally.GetMove(0).GetCurrentPowerPoints() + "/" + ally.GetMove(0).GetPowerPoints();
            else
            {
                firstMovePP.Text = string.Empty;
                firstMove.Click -= Attack;
            }
            if (ally.GetMove(1).GetName() != string.Empty)
                secondMovePP.Text = ally.GetMove(1).GetCurrentPowerPoints() + "/" + ally.GetMove(1).GetPowerPoints();
            else
            {
                secondMovePP.Text = string.Empty;
                secondMove.Click -= Attack;
            }
            if (ally.GetMove(2).GetName() != string.Empty)
                thirdMovePP.Text = ally.GetMove(2).GetCurrentPowerPoints() + "/" + ally.GetMove(2).GetPowerPoints();
            else
            {
                thirdMovePP.Text = string.Empty;
                thirdMove.Click -= Attack;
            }
            if (ally.GetMove(3).GetName() != string.Empty)
                fourthMovePP.Text = ally.GetMove(3).GetCurrentPowerPoints() + "/" + ally.GetMove(3).GetPowerPoints();
            else
            {
                fourthMovePP.Text = string.Empty;
                fourthMove.Click -= Attack;
            }

            //We change the background color of the button, each type has a different color.
            //We get the type of the move, its respective index, then the colour and we finally apply it to the button.
            SetButtonColor(firstMove, GetIndex(ally.GetMoveType(0)));
            SetButtonColor(secondMove, GetIndex(ally.GetMoveType(1)));
            SetButtonColor(thirdMove, GetIndex(ally.GetMoveType(2)));
            SetButtonColor(fourthMove, GetIndex(ally.GetMoveType(3)));

            //Update the image.
            ChangeAllyImage(ally.GetBackSprite());

            //Update health values.
            playerLife.Maximum = ally.GetSpecie().GetHP();
            playerLife.Value = ally.GetCurrentHP();
            UpdateHealthBar(ally, 0);

            //Update name.
            allyName.Text = ally.GetNickname();

            //Update gender symbol.
            if (ally.GetGender() == Gender.MALE)
            {
                allyGender.Text = "♂";
                allyGender.Foreground = Brushes.LightBlue;
                allyGenderEffect.Color = Colors.Blue;
            }
            else if (ally.GetGender() == Gender.FEMALE)
            {
                allyGender.Text = "♀";
                allyGender.Foreground = Brushes.Pink;
                allyGenderEffect.Color = Colors.DarkMagenta;
            }
            else
                allyGender.Text = ""; //Genderless
        }

        private void FightClick(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //We change the panel visibility to display the moves our pokémon has available.
            mainPanel.Visibility = Visibility.Collapsed;
            movePanel.Visibility = Visibility.Visible;
        }

        private void BagClick(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Enter a new battle
            this.NavigationService.Navigate(new Bag(trainer, false, true));
            bgm.Stop();
        }

        private void PokémonClick(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //We change the panel visibility to display the our pokémon.
            mainPanel.Visibility = Visibility.Collapsed;
            pokemonPanel.Visibility = Visibility.Visible;
        }

        private async void RunClick(object sender, RoutedEventArgs e)
        {
            mainPanel.Visibility = Visibility.Collapsed;
            if (ally.Escape(r, enemy, flightAttempts) == true)
            {
                //Play the run away sound effect.
                runAwaySound.Play();
                runAwaySound.Position = TimeSpan.Zero;

                //Update text.
                displayInfo.Text = "Got away safely.";
                await Task.Delay(1000);

                //Go to the main menu.
                this.NavigationService.GoBack();
                bgm.Stop();
            }
            else
            {
                flightAttempts += 1;
                displayInfo.Text = "You couldn't get away!";

                await Task.Delay(1000); //Makes the program easier to read.

                //The enemy attacks our pokémon. We stop the method if they defeat us.
                if (await EnemyAttack(enemy.SelectRandomMove(r)))
                    return;

                await Task.Delay(1000);

                displayInfo.Text = "What will " + ally.GetNickname() + " do?";

                //Change the panels back.
                mainPanel.Visibility = Visibility.Visible;
            }
        }

        private async void Attack(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Hide the panel    
            movePanel.Visibility = Visibility.Collapsed;

            //First we need to know which move did the user select.
            Button clickedButton = sender as Button;
            Move selectedMove;
            string selectedMovePP; //Used to know which move we have to visually update.

            switch (clickedButton.Name)
            {
                case "firstMove":
                    {
                        selectedMove = ally.GetMove(0);
                        selectedMovePP = "firstMovePP";
                        break;
                    }
                case "secondMove":
                    {
                        selectedMove = ally.GetMove(1);
                        selectedMovePP = "secondMovePP";
                        break;
                    }
                case "thirdMove":
                    {
                        selectedMove = ally.GetMove(2);
                        selectedMovePP = "thirdMovePP";
                        break;
                    }
                case "fourthMove":
                    {
                        selectedMove = ally.GetMove(3);
                        selectedMovePP = "fourthMovePP";
                        break;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid variable", nameof(clickedButton.Name));
                    }
            }

            if (selectedMove.GetCurrentPowerPoints() == 0)
            {
                displayInfo.Text = "There's no PP left for this move!";

                await Task.Delay(1000);
                displayInfo.Text = "What will " + ally.GetNickname() + " do?";

                mainPanel.Visibility = Visibility.Visible;
                return;
            }

            Move randomMove = enemy.SelectRandomMove(r);

            if (selectedMove.GetPriority() > randomMove.GetPriority()) //We attack first.
            {
                //We attack the enemy. We stop the method if we defeat them.
                if (await PlayerAttack(selectedMove, selectedMovePP))
                    return;

                await Task.Delay(1000); //Makes the program easier to read.

                //The enemy attacks our pokémon. We stop the method if they defeat us.
                if (await EnemyAttack(randomMove))
                    return;
            }

            else if (selectedMove.GetPriority() < randomMove.GetPriority()) // Enemy attacks first.
            {
                //The enemy attacks our pokémon. We stop the method if they defeat us.
                if (await EnemyAttack(randomMove))
                    return;

                await Task.Delay(1000); //Makes the program easier to read.

                //We attack the enemy. We stop the method if we defeat them.
                if (await PlayerAttack(selectedMove, selectedMovePP))
                    return;
            }

            //Now we check the speed.
            else if (ally.GetSpecie().GetSpeed() >= enemy.GetSpecie().GetSpeed()) //We are faster.
            {
                //We attack the enemy. We stop the method if we defeat them.
                if (await PlayerAttack(selectedMove, selectedMovePP))
                    return;

                await Task.Delay(1000); //Makes the program easier to read.

                //The enemy attacks our pokémon. We stop the method if they defeat us.
                if (await EnemyAttack(randomMove))
                    return;
            }
            else //They are faster.
            {
                //The enemy attacks our pokémon. We stop the method if they defeat us.
                if (await EnemyAttack(randomMove))
                    return;

                await Task.Delay(1000); //Makes the program easier to read.

                //We attack the enemy. We stop the method if we defeat them.
                if (await PlayerAttack(selectedMove, selectedMovePP))
                    return;
            }

            await Task.Delay(1000);

            displayInfo.Text = "What will " + ally.GetNickname() + " do?";

            //Change the panels back.
            mainPanel.Visibility = Visibility.Visible;
        }

        private void UpdateHealthBar(Pokémon pokemon, int damage)
        {
            if (pokemon == enemy)
            {
                enemyLife.Value -= damage;
                int percentage = (int)Math.Round(enemyLife.Value * 100 / enemyLife.Maximum);
                if (percentage <= 20)
                    enemyLife.Foreground = new SolidColorBrush(Colors.Red);
                else if (percentage <= 50)
                    enemyLife.Foreground = new SolidColorBrush(Colors.Yellow);
                
            }
            else
            {
                playerLife.Value -= damage;
                int percentage = (int)Math.Round(playerLife.Value * 100 / playerLife.Maximum);
                if (percentage <= 20)
                    playerLife.Foreground = new SolidColorBrush(Colors.Red);
                else if (percentage <= 50)
                    playerLife.Foreground = new SolidColorBrush(Colors.Yellow);
                else
                    playerLife.Foreground = new SolidColorBrush(Colors.LimeGreen);

                playerNumericHealth.Text = ally.GetCurrentHP() + "/" + ally.GetSpecie().GetHP();
            }
        }

        private async void DefeatPokemon(Pokémon pokemon)
        {
            movePanel.Visibility = Visibility.Collapsed;
            if (pokemon == enemy)
            {             
                displayInfo.Text = "The " + pokemon.GetNickname() + " enemy has been defeated!";
                ChangeEnemyImage(string.Empty);

                await Task.Delay(1000);

                DistributeRewards();
                enemy = null;

                await Task.Delay(1000);
                displayInfo.Text = "Please return to the main menu.";

                returnPanel.Visibility = Visibility.Visible;
            }
            else
            {
                displayInfo.Text = "Oh no! Your " + pokemon.GetNickname() + " has fainted.";

                await Task.Delay(1000);

                //We will do something different depending on if it is our last pokémon or not.
                if (AtLeastOnePokemon() == true) //We keep fighting. 
                { 
                    ally = null;
                    ChangeAllyImage(string.Empty);
                    UpdatePokemonPanel();                   
                    mainPanel.Visibility = Visibility.Collapsed; //When you change a pokémon and it gets defeated in that turn we need to close this.
                    pokemonPanel.Visibility = Visibility.Visible;

                    //We cannot run away after they have defeated our pokémon.
                    returnToMainPanel.MouseDown -= ReturnMainPanel_Click;
                    returnToMainPanel.MouseDown += Warning;

                    displayInfo.Text = "Choose a pokémon to send to battle.";
                }
                else //We need to return to the menu.
                {
                    ally = null;
                    ChangeAllyImage(string.Empty);
                    returnPanel.Visibility = Visibility.Visible;
                }                
            }
        }

        private void DistributeRewards()
        {
            int increase = enemy.GetRewardRate();
            displayInfo.Text = "The " + enemy.GetNickname() + " enemy has dropped ¥" + increase + ".";
            trainer.GetWallet().AddDollars(increase);
        }

        private bool AtLeastOnePokemon()
        {
            int counter = 0;
            for (int i = 0; i < 5; i += 1)
            {
                if (trainer.GetPokemonInTeam(i) == null)
                    break;
                if (trainer.GetPokemonInTeam(i).GetCurrentHP() > 0)
                    counter += 1;
            }
            return counter > 0;
        }

        private void ChangeAllyImage(string newSource)
        {
            //Used to change the pokémon image
            allyPokémon.Source = new BitmapImage(new Uri(newSource, UriKind.Relative));
        }

        private void ChangeEnemyImage(string newSource)
        {
            //Used to change the pokémon image
            wildPokémon.Source = new BitmapImage(new Uri(newSource, UriKind.Relative));
        }

        private async void GenerateNewWildPokémon()
        {
            //We create and initialize a random wild pokémon.
            enemy = new Pokémon(r);

            //Update their image.
            ChangeEnemyImage(enemy.GetSpecie().GetFrontSprite());

            //Update life bar.
            enemyLife.Maximum = enemy.GetSpecie().GetHP();
            enemyLife.Value = enemy.GetSpecie().GetHP();

            //Update name.
            wildNameBlock.Text = enemy.GetSpecie().GetSpeciesName();

            //Update gender symbol.
            if (enemy.GetGender() == Gender.MALE)
            {
                wildGender.Text = "♂";
                wildGender.Foreground = Brushes.LightBlue;
                wildGenderEffect.Color = Colors.Blue;
            }
            else if (enemy.GetGender() == Gender.FEMALE)
            {
                wildGender.Text = "♀";
                wildGender.Foreground = Brushes.Pink;
                wildGenderEffect.Color = Colors.DarkMagenta;
            }
            else
                wildGender.Text = ""; //Genderless

            //Update textblock.
            displayInfo.Text = "A wild " + enemy.GetSpecie().GetSpeciesName() + " appeared!";

            await Task.Delay(1000);

            displayInfo.Text = "What will " + ally.GetNickname() + " do?";

            mainPanel.Visibility = Visibility.Visible;
        }

        private int GetIndex(Enum value)
        {
            //Used to get the index value of an element inside an Enum (used for Type enum).
            Array values = Enum.GetValues(value.GetType());
            return Array.IndexOf(values, value);
        }

        private void SetButtonColor(Button button, int index)
        {
            //Used to change the background color of a button (for moves).
            string colorString = GetMoveTypeColour(index);
            SolidColorBrush brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorString));
            button.Background = brush;
        }

        private string GetMoveTypeColour(int index)
        {
            //Used to select a color depending on the type of the move.
            string[] moveTypeColour =
            {
                "#A6A6A6", //NORMAL
                "#EA0A22", //FIRE
                "#1F7DF7", //WATER
                "#44DB1B", //GRASS
                "#F5C32E", //ELECTRIC
                "#42EDEA", //ICE
                "#D97C21", //FIGHTING
                "#AD47B3", //POISON
                "#B2886A", //GROUND
                "#E0FFFF", //FLYING
                "#FF8DF7", //PSYCHIC
                "#3EC473", //BUG
                "#C4C49E", //ROCK
                "#B39BC4", //GHOST
                "#771482", //DRAGON
                "#58135E", //DARK
                "#ACC7C0", //STEEL
                "#FA42FA", //FAIRY
                "#FFFFFF", //NULL
            };

            return moveTypeColour[index];
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
            bgm.Stop();
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

        private async void PokemonCell_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //Changes the current pokemon.

            //This tells the difference between changing during the combat, or after our pokémon has been defeated.
            bool isDefeated;
            isDefeated = trainer.GetPokemonInTeam(0).GetCurrentHP() <= 0;

            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Check that the clicked cell has an alive pokémon.
            //Get the clicked cell
            Border clickedCell = sender as Border;

            if (clickedCell.Background == (SolidColorBrush)(new BrushConverter().ConvertFrom("DarkOrange")))
            {
                //Is not alive.
                displayInfo.Text = "This pokémon has no energy left to battle!";
                return;
            }
            else if (clickedCell.Name == "firstPokemon" && ally.GetCurrentHP() > 0)
            {
                //We cannot change with the current pokemon.
                displayInfo.Text = ally.GetNickname() + " is already in battle!";
                return;
            }
            else
            {
                //We need to change the pokémon.
                pokemonPanel.Visibility = Visibility.Collapsed;

                //Which pokémon have we clicked.
                int secondIndex = VariableToInt(clickedCell.Name);

                //Swap the current pokémon with the new one. Code-wise.
                trainer.SwapPokemonTeamPositions(0, secondIndex);
                ally = trainer.GetPokemonInTeam(0); //The pokemon in the first position is now the one is fighting.

                UpdateAllyUI(); //We update the visual aspect.
                displayInfo.Text = "Go! " + ally.GetNickname() + "!";
                UpdatePokemonPanel();

                await Task.Delay(1000);

                if (!isDefeated) //The enemy needs to attack our pokémon.
                {
                    //The enemy attacks our pokémon, as we lose the turn changing between pokémon.
                    //We stop the method if they defeat us.
                    if (await EnemyAttack(enemy.SelectRandomMove(r)))
                        return;

                    await Task.Delay(1000);
                }

                //We restore the back button.
                returnToMainPanel.MouseDown -= Warning;
                returnToMainPanel.MouseDown += ReturnMainPanel_Click;

                mainPanel.Visibility = Visibility.Visible;
                displayInfo.Text = "What will " + ally.GetNickname() + " do?";
            }
        }

        private async Task<bool> EnemyAttack(Move randomMove)
        {
            // The enemy attacks our Pokémon. Returns true if they defeat us.
            int damage = enemy.Attack(r, ally, randomMove);
            displayInfo.Text = "The wild " + enemy.GetNickname() + " used " + randomMove.GetName() + ".";

            if (damage == 0) //The attack was not compatible.
            {
                await Task.Delay(1000);
                displayInfo.Text = "It had no effect on " + ally.GetNickname() + ".";
            }
            else if (damage == -1) //Attack misses.
            {
                await Task.Delay(1000);
                displayInfo.Text = "But it failed.";
                damage = 0;
            }
            else //The attack hits.
            {
                displayInfo.Text += GetCompatibilityMessage(randomMove.GetMoveType(), ally.GetFirstType(), ally.GetSecondType());
                UpdateHealthBar(ally, damage);

                if (playerLife.Value <= 0) // They have defeated us.
                {
                    await Task.Delay(1000);
                    DefeatPokemon(ally);
                    return true;
                }
            }
            return false;
        }

        private async Task<bool> PlayerAttack(Move selectedMove, string selectedMovePP)
        {
            //We attack the enemy. Returns true if we defeat them.
            int damage = ally.Attack(r, enemy, selectedMove);
            displayInfo.Text = ally.GetNickname() + " used " + selectedMove.GetName() + ".";

            //We need to visually update the PP.
            switch (selectedMovePP)
            {
                case "firstMovePP":
                    {
                        firstMovePP.Text = ally.GetMove(0).GetCurrentPowerPoints() + "/" + ally.GetMove(0).GetPowerPoints();
                        break;
                    }
                case "secondMovePP":
                    {
                        secondMovePP.Text = ally.GetMove(1).GetCurrentPowerPoints() + "/" + ally.GetMove(1).GetPowerPoints();
                        break;
                    }
                case "thirdMovePP":
                    {
                        thirdMovePP.Text = ally.GetMove(2).GetCurrentPowerPoints() + "/" + ally.GetMove(2).GetPowerPoints();
                        break;
                    }
                case "fourthMovePP":
                    {
                        fourthMovePP.Text = ally.GetMove(3).GetCurrentPowerPoints() + "/" + ally.GetMove(3).GetPowerPoints();
                        break;
                    }
            }

            if (damage == 0) //Not compatible.
            {
                await Task.Delay(1000);
                displayInfo.Text = "It had no effect on the enemy " + enemy.GetNickname() + ".";
            }
            else if (damage == -1) //Attack misses.
            {
                await Task.Delay(1000);
                displayInfo.Text = "But it failed.";
                damage = 0;
            }
            else //Attack hits.
            {
                displayInfo.Text += GetCompatibilityMessage(selectedMove.GetMoveType(), enemy.GetFirstType(), enemy.GetSecondType());
                UpdateHealthBar(enemy, damage);

                if (enemyLife.Value <= 0) //We have defeated it.
                {
                    await Task.Delay(1000);
                    DefeatPokemon(enemy);
                    return true;
                }
            }
            return false;
        }

        private void Warning(object sender, RoutedEventArgs e) { displayInfo.Text = "You can't do that right now!"; }

        private void ReturnMainPanel_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //We change the panel visibility to display the our pokémon.
            pokemonPanel.Visibility = Visibility.Collapsed;
            movePanel.Visibility = Visibility.Collapsed;
            mainPanel.Visibility = Visibility.Visible;

            displayInfo.Text = "What will " + ally.GetNickname() + " do?";
        }

        private int VariableToInt(string name)
        {
            switch (name)
            {
                case "firstPokemon":
                    return 0;
                case "secondPokemon":
                    return 1;
                case "thirdPokemon":
                    return 2;
                case "fourthPokemon":
                    return 3;
                case "fifthPokemon":
                    return 4;
                case "sixthPokemon":
                    return 5;
                default:
                    throw new ArgumentException("Invalid variable", nameof(name));
            }
        }

        private string GetCompatibilityMessage(Type moveType, Type firstDefenderType, Type secondDefenderType)
        {
            float compatibility = ally.GetSpecie().GetCompatibility(moveType, firstDefenderType);
            float secondCompatibility = ally.GetSpecie().GetCompatibility(moveType, secondDefenderType);
            float effectiveness = compatibility * secondCompatibility;
            
            switch (effectiveness)
            {
                case 0:
                    {
                        return string.Empty;
                    }
                case 0.25f:
                    {
                        return " It's extremely ineffective...";
                    }
                case 0.5f:
                    {
                        return " It's not very effective...";
                    }
                case 1:
                    {
                        return string.Empty;
                    }
                case 2:
                    {
                        return " It's very effective!";
                    }
                case 4:
                    {
                        return " It's extremely effective!";
                    }
                default:
                    {
                        throw new ArgumentException();
                    }
            }
        }
    }
}
