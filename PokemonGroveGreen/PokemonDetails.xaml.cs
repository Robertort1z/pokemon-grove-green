using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Lógica de interacción para PokemonDetails.xaml
    /// </summary>
    public partial class PokemonDetails : Page
    {
        //Author: Roberto Ortiz Perez
        MediaPlayer buttonSound = new MediaPlayer();
        Pokémon pokemon; //Send to move details.

        public PokemonDetails(Pokémon pokemon)
        {
            InitializeComponent();
            this.pokemon = pokemon;

            //We prepare the sound effect so there is less delay after pressing the button.
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));

            UpdatePokemon(pokemon);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
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

        private void UpdatePokemon(Pokémon pokemon)
        {
            //Update the image.
            pokemonImage.Source = new BitmapImage(new Uri(pokemon.GetFrontSprite(), UriKind.Relative));

            //Update the number.
            numberLabel.Content = pokemon.GetSpeciesNumber().ToString("000");

            //Update the name.
            nameLabel.Content = pokemon.GetNickname();

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

            //Update the title.
            titleLabel.Content = pokemon.GetTitle() + " Pokémon";

            //Update the types.
            firstType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetFirstType()), UriKind.Relative));
            secondType.Source = new BitmapImage(new Uri(GetTypeSource(pokemon.GetSecondType()), UriKind.Relative));

            //Update health.
            hpLabel.Content = "Health: " + pokemon.GetCurrentHP().ToString() + "/" + pokemon.GetHP().ToString();

            //Update EO.
            eoLabel.Content = "EO: " + pokemon.GetEO().GetName();

            //Update info
            infoBlockHP.Text = "HP: " + pokemon.GetHP().ToString();
            infoBlockAtk.Text = "Atk: " + pokemon.GetAttack().ToString();
            infoBlockDef.Text = "Def: " + pokemon.GetDefense().ToString() + "\n"; //Line change
            infoBlockSpAtk.Text = "SpAtk: " + pokemon.GetSpAtk().ToString();
            infoBlockSpDef.Text = "SpDef: " + pokemon.GetSpDef().ToString();
            infoBlockSpeed.Text = "Speed: " + pokemon.GetSpeed().ToString();
        }

        private void MoveDetails_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Show details.
            this.NavigationService.Navigate(new MoveDetails(pokemon));
        }
    }
}
