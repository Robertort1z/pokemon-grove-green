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
using static System.Net.Mime.MediaTypeNames;

namespace PokemonGroveGreen
{
    /// <summary>
    /// Lógica de interacción para MoveDetails.xaml
    /// </summary>
    public partial class MoveDetails : Page
    {
        //Author: Roberto Ortiz Perez
        MediaPlayer buttonSound = new MediaPlayer();
        Pokémon pokemon;

        public MoveDetails(Pokémon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;

            UpdateData();

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
        }

        private void UpdateData()
        {
            //Pokemon image
            pokemonImage.Source = new BitmapImage(new Uri(pokemon.GetFrontSprite(), UriKind.Relative));

            //Update the name.
            nameLabel.Content = pokemon.GetSpecie().GetSpeciesName();

            //Update gender.
            if (pokemon.GetGender() == Gender.MALE)
            {
                genderBlock.Text = "♂";
                genderBlock.Foreground = Brushes.DarkBlue;
                genderEffect.Color = Colors.Blue;
            }
            else if (pokemon.GetGender() == Gender.FEMALE)
            {
                genderBlock.Text = "♀";
                genderBlock.Foreground = Brushes.DarkMagenta;
                genderEffect.Color = Colors.Pink;
            }
            else
                genderBlock.Text = ""; //Genderless

            //Update the types.
            firstType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetFirstType()), UriKind.Relative));
            secondType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetSecondType()), UriKind.Relative));

            //Temporarily remove kind.
            categoryImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));

            //Update move names.
            firstMoveName.Content = pokemon.GetMove(0).GetName();
            secondMoveName.Content = pokemon.GetMove(1).GetName();
            thirdMoveName.Content = pokemon.GetMove(2).GetName();
            fourthMoveName.Content = pokemon.GetMove(3).GetName();

            //Update move types.
            firstMoveType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetMove(0).GetMoveType()), UriKind.Relative));
            secondMoveType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetMove(1).GetMoveType()), UriKind.Relative));
            thirdMoveType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetMove(2).GetMoveType()), UriKind.Relative));
            fourthMoveType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetMove(3).GetMoveType()), UriKind.Relative));

            //Update move PP.
            if (pokemon.GetMove(0).GetName() != string.Empty) 
                firstMovePp.Content = "PP:" + pokemon.GetMove(0).GetCurrentPowerPoints() + "/" + pokemon.GetMove(0).GetPowerPoints();
            else
                firstMovePp.Content = string.Empty;
            if (pokemon.GetMove(1).GetName() != string.Empty)
                secondMovePp.Content = "PP:" + pokemon.GetMove(1).GetCurrentPowerPoints() + "/" + pokemon.GetMove(1).GetPowerPoints();
            else
                secondMovePp.Content = string.Empty;
            if (pokemon.GetMove(2).GetName() != string.Empty)
                thirdMovePp.Content = "PP:" + pokemon.GetMove(2).GetCurrentPowerPoints() + "/" + pokemon.GetMove(2).GetPowerPoints();
            else
                thirdMovePp.Content = string.Empty;
            if (pokemon.GetMove(3).GetName() != string.Empty)
                fourthMovePp.Content = "PP:" + pokemon.GetMove(3).GetCurrentPowerPoints() + "/" + pokemon.GetMove(3).GetPowerPoints();
            else
                fourthMovePp.Content = string.Empty;
        }

        private string GetTypeSource(Type type)
        {
            switch (type)
            {
                case Type.NORMAL:
                    {
                        return "/Images/normalType.png";
                    }
                case Type.FIRE:
                    {
                        return "/Images/fireType.png";
                    }
                case Type.WATER:
                    {
                        return "/Images/waterType.png";
                    }
                case Type.GRASS:
                    {
                        return "/Images/grassType.png";
                    }
                case Type.ELECTRIC:
                    {
                        return "/Images/electricType.png";
                    }
                case Type.ICE:
                    {
                        return "/Images/iceType.png";
                    }
                case Type.FIGHTING:
                    {
                        return "/Images/fightingType.png";
                    }
                case Type.POISON:
                    {
                        return "/Images/poisonType.png";
                    }
                case Type.GROUND:
                    {
                        return "/Images/groundType.png";
                    }
                case Type.FLYING:
                    {
                        return "/Images/flyingType.png";
                    }
                case Type.PSYCHIC:
                    {
                        return "/Images/psychicType.png";
                    }
                case Type.BUG:
                    {
                        return "/Images/bugType.png";
                    }
                case Type.ROCK:
                    {
                        return "/Images/rockType.png";
                    }
                case Type.GHOST:
                    {
                        return "/Images/ghostType.png";
                    }
                case Type.DRAGON:
                    {
                        return "/Images/dragonType.png";
                    }
                case Type.DARK:
                    {
                        return "/Images/darkType.png";
                    }
                case Type.STEEL:
                    {
                        return "/Images/steelType.png";
                    }
                case Type.FAIRY:
                    {
                        return "/Images/fairyType.png";
                    }
                case Type.NULL:
                    {
                        return string.Empty;
                    }
                default:
                    {
                        throw new ArgumentException("Invalid variable", nameof(type));
                    }
            }
        }

        private void Cell_MouseEnter(object sender, MouseEventArgs e)
        {
            // Change cursor to hand when mouse enters the cell
            Mouse.OverrideCursor = Cursors.Hand;

            //Update border.
            //Border hoveredBorder = sender as Border;
            //hoveredBorder.BorderThickness = new Thickness(1);
        }

        private void Cell_MouseLeave(object sender, MouseEventArgs e)
        {
            // Reset cursor when mouse leaves the cell
            Mouse.OverrideCursor = null;

            //Get which border we un-hovered.
            //Border hoveredBorder = sender as Border;

            //Update border.
            //hoveredBorder.BorderThickness = new Thickness(0);
        }

        private void Back_Click(object sender, MouseButtonEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
        }

        private void Move_MouseEnter(object sender, MouseEventArgs e)
        {
            //Get which move we need to display
            Border hoveredBorder = sender as Border;
            Move move = GetMoveWithBorder(hoveredBorder);

            if (move == null)
                return;

            //Update category
            if (move.GetKind() == Kind.PHYSICAL)
                categoryImage.Source = new BitmapImage(new Uri("/Images/physicalKind.png", UriKind.Relative));
            else if (move.GetKind() == Kind.SPECIAL)
                categoryImage.Source = new BitmapImage(new Uri("/Images/specialKind.png", UriKind.Relative));
            else
                categoryImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));

            //Update power
            if (move.GetName() != string.Empty)
                powerBlock.Text = move.GetPower().ToString();
            else
                powerBlock.Text = string.Empty;

            //Update accuracy
            if (move.GetName() != string.Empty)
                accuracyBlock.Text = move.GetAccuracy().ToString();
            else
                accuracyBlock.Text = string.Empty;           

            //Update description
            moveDescription.Text = move.GetDescription();

            //Update border.
            hoveredBorder.BorderThickness = new Thickness(1);
        }

        private void Move_MouseLeave(object sender, MouseEventArgs e)
        {
            //Get which border we un-hovered.
            Border hoveredBorder = sender as Border;

            //Update border.
            hoveredBorder.BorderThickness = new Thickness(0);

            //Remove move info.
            categoryImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));
            powerBlock.Text = string.Empty;
            accuracyBlock.Text = string.Empty;
            moveDescription.Text = string.Empty;
        }

        private Move GetMoveWithBorder(Border hoveredBorder)
        {
            switch (hoveredBorder.Name)
            {
                case "firstMove":
                    {
                        return pokemon.GetMove(0);
                    }
                case "secondMove":
                    {
                        return pokemon.GetMove(1);
                    }
                case "thirdMove":
                    {
                        return pokemon.GetMove(2);
                    }
                case "fourthMove":
                    {
                        return pokemon.GetMove(3);
                    }
                default:
                    {
                        throw new ArgumentException("Invalid variable", nameof(hoveredBorder));
                    }
            }
        }
    }
}
