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
    /// Lógica de interacción para ShopBuy.xaml
    /// </summary>
    public partial class ShopBuy : Page
    {
        //Author: Roberto Ortiz Perez
        Trainer trainer;
        MediaPlayer buttonSound = new MediaPlayer();
        Item displayedItem;
        int displayedItemIndex;
        static readonly Item[] availableItems = new Item[]
        {
            new Potion(1),
            new Potion(2),
            new Potion(3),
            new Potion(4),
            new Revive(5),
            new Revive(6),
            new Ether(7),
            new Ether(8),
            new Ether(9),
            new Ether(10),
            new EvolutionStone(11),
            new EvolutionStone(12),
            new EvolutionStone(13),
            new EvolutionStone(14),
            new EvolutionStone(15),
            new EvolutionStone(16),
            new EvolutionStone(17),
            new EvolutionStone(18),
            new Pokeball(19),
            new Pokeball(20),
            new Pokeball(21),
            new Pokeball(22),
            new Pokedoll(),
            new TM(23),
            new TM(24),
            new TM(25),
            new TM(26),
            new TM(27),
            new TM(28),
            new Berry(29),
            new Berry(30),
            new Berry(31),
            new Treasure(32),
            new Treasure(33),
            new Treasure(34),
            new Treasure(35),
            new Treasure(36),
            new Treasure(37),
            new Treasure(38),
            new Treasure(39),
            new Treasure(40),
        };

        public ShopBuy(Trainer trainer)
        {
            InitializeComponent();
            this.trainer = trainer;
            displayedItemIndex = 0;
            displayedItem = availableItems[displayedItemIndex];
            DisplayItemInfo(displayedItemIndex);
            buttonSound.Open(new Uri("../../MusicAndSoundEffects/ButtonSound.mp3", UriKind.Relative));
            shopBuyViewbox.Focus();
        }

        private async void ShopViewbox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                    {
                        MoveShopRight();
                        //Update the item we want to purchase.
                        displayedItem = availableItems[displayedItemIndex];
                        price.Text = availableItems[displayedItemIndex].GetCostString();
                        break;
                    }
                case Key.A:
                    {
                        MoveShopLeft();
                        //Update the item we want to purchase.
                        displayedItem = availableItems[displayedItemIndex];
                        price.Text = availableItems[displayedItemIndex].GetCostString();
                        break;
                    }
                case Key.Enter:
                    {
                        if (MaxItemAllowed() >= 1)
                            BuyItem();
                        else
                        {
                            DisplayMessage("You don't have enough money.");
                            await Task.Delay(1000);
                            messageBorder.BorderThickness = new Thickness(0);
                            messagePanel.Visibility = Visibility.Collapsed;
                        }
                        break;
                    }
                case Key.Escape:
                    {
                        this.NavigationService.GoBack();
                        break;
                    }
            }
        }

        private void MoveShopLeft()
        {
            if (displayedItemIndex == 0)
                displayedItemIndex = availableItems.Length - 1;
            else
                displayedItemIndex -= 1;
            DisplayItemInfo(displayedItemIndex);
        }

        private void MoveShopRight()
        {
            if (displayedItemIndex == availableItems.Length - 1)
                displayedItemIndex = 0;
            else
                displayedItemIndex += 1;
            DisplayItemInfo(displayedItemIndex);
        }

        int itemAmount;
        int maxItem;
        private void BuyItem()
        {
            maxItem = MaxItemAllowed();

            itemAmount = 1;
            shopBuyViewbox.PreviewKeyDown -= ShopViewbox_KeyDown;
            shopBuyViewbox.PreviewKeyDown += Confirmation_KeyDown;

            amount.Text = "← " + itemAmount + " →";
            price.Text = availableItems[displayedItemIndex].GetCostString();

            //Show confirmation menu.
            confirmationPanel.Visibility = Visibility.Visible;
            confirmationBorder.BorderThickness = new Thickness(5);
        }

        public void DisplayItemInfo(int itemIndex)
        {
            //Update the image.
            string newSource = availableItems[itemIndex].GetSprite();
            itemImage.Source = new BitmapImage(new Uri(newSource, UriKind.Relative));

            //Update object name.
            itemTitle.Text = availableItems[itemIndex].GetName();

            //Update details.
            infoBlock.Text = availableItems[itemIndex].ToString();
        }

        private int MaxItemAllowed()
        {
            int format = DetectFormatType(price.Text);
            switch (format)
            {
                case 0: //PokeDollars
                    {
                        double max = trainer.GetPokeDollars() / displayedItem.GetCost();
                        return (int)Math.Truncate(max);
                    }
                case 1: //BattlePoints
                    {
                        double max = trainer.GetBattlePoints() / displayedItem.GetCost();
                        return (int)Math.Truncate(max);
                    }
                case 2: //PokeMiles
                    {
                        double max = trainer.GetPokeMiles() / displayedItem.GetCost();
                        return (int)Math.Truncate(max);
                    }
            }
            throw new ArgumentException();
        }

        private void Confirmation_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        ReturnToShop();
                        break;
                    }
                case Key.D:
                    {
                        if (itemAmount == maxItem) // || x until 999
                            break;
                        itemAmount += 1;
                        amount.Text = "← " + itemAmount + " →";
                        price.Text = MultiplyPrice(price.Text, itemAmount);
                        break;
                    }
                case Key.A:
                    {
                        if (itemAmount == 1)
                            break;
                        itemAmount -= 1;
                        amount.Text = "← " + itemAmount + " →";
                        price.Text = MultiplyPrice(price.Text, itemAmount);
                        break;
                    }
                case Key.Enter:
                    {
                        ConfirmPurchase();
                        break;
                    }
            }
        }

        private void ConfirmPurchase()
        {
            //Hide confirmation menu.
            confirmationBorder.BorderThickness = new Thickness(0);
            confirmationPanel.Visibility = Visibility.Collapsed;
            shopBuyViewbox.PreviewKeyDown -= Confirmation_KeyDown;
            //Show buy menu.
            buyPanel.Visibility = Visibility.Visible;
            buyBorder.BorderThickness = new Thickness(5);
            buyPrice.Text = "That will be " + price.Text + ". Okay?";
        }

        private string MultiplyPrice(string price, int multiplier)
        {
            //Checks the format.
            if (price.StartsWith("¥"))
            {
                //PokeDollars
                return "¥" + (availableItems[displayedItemIndex].GetCost() * multiplier);
            }
            else if (price.EndsWith(" BP"))
            {
                //BattlePoints
                return (availableItems[displayedItemIndex].GetCost() * multiplier) + " BP";
            }
            else if (price.EndsWith(" PM"))
            {
                //PokeMiles
                return (availableItems[displayedItemIndex].GetCost() * multiplier) + " PM";
            }

            //If the format does not match with any of the expected ones.
            throw new ArgumentException("The price format is not valid.", nameof(price));
        }

        private void ReturnToShop()
        {
            confirmationPanel.Visibility = Visibility.Collapsed;
            confirmationBorder.BorderThickness = new Thickness(0);

            buyPanel.Visibility = Visibility.Collapsed;
            buyBorder.BorderThickness = new Thickness(0);

            messagePanel.Visibility = Visibility.Collapsed;
            messageBorder.BorderThickness = new Thickness(0);

            shopBuyViewbox.PreviewKeyDown -= Confirmation_KeyDown;
            shopBuyViewbox.PreviewKeyDown += ShopViewbox_KeyDown;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            //Go to the main menu.
            this.NavigationService.GoBack();
        }

        private async void Purchase_Click(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            int finalPrice = ExtractIntegerValue(price.Text);
            int format = DetectFormatType(price.Text);
            int quantity = finalPrice / displayedItem.GetCost();

            switch (format)
            {
                case 0: //PokeDollars
                    {
                        //Buy item.
                        AddItemToBag(quantity);
                        trainer.GetWallet().DeductDollars(finalPrice);
                        DisplayMessage("Here you are. Thank you!");
                        await Task.Delay(1000);
                        ReturnToShop();
                        break;
                    }
                case 1: //BattlePoints
                    {
                        //Buy item.
                        AddItemToBag(quantity);
                        trainer.GetWallet().DeductBattlePoints(finalPrice);
                        DisplayMessage("Here you are. Thank you!");
                        await Task.Delay(1000);
                        ReturnToShop();
                        break;
                    }
                case 2: //PokeMiles
                    {
                        //Buy item.
                        AddItemToBag(quantity);
                        trainer.GetWallet().DeductPokeMiles(finalPrice);
                        DisplayMessage("Here you are. Thank you!");
                        await Task.Delay(1000);
                        ReturnToShop();
                        break;
                    }
            }
        }

        private void DisplayMessage(string newMessage)
        {
            buyPanel.Visibility = Visibility.Collapsed;
            buyBorder.BorderThickness = new Thickness(0);

            message.Text = newMessage;
            messageBorder.BorderThickness = new Thickness(5);
            messagePanel.Visibility = Visibility.Visible;
        }

        private void AddItemToBag(int quantity)
        {
            switch (displayedItem)
            {
                case EvolutionStone _:
                    {
                        trainer.GetBag().Add(0, displayedItem, quantity);
                        break;
                    }
                case Medicine _:
                    {
                        trainer.GetBag().Add(1, displayedItem, quantity);
                        break;
                    }
                case Pokeball _:
                    {
                        trainer.GetBag().Add(2, displayedItem, quantity);
                        break;
                    }
                case TM _:
                    {
                        trainer.GetBag().Add(3, displayedItem, quantity);
                        break;
                    }
                case Berry _:
                    {
                        trainer.GetBag().Add(4, displayedItem, quantity);
                        break;
                    }
                case Treasure _:
                    {
                        trainer.GetBag().Add(5, displayedItem, quantity);
                        break;
                    }
                case Pokedoll _:
                    {
                        trainer.GetBag().Add(6, displayedItem, quantity);
                        break;
                    }
            }
        }

        private int ExtractIntegerValue(string input)
        {
            string numberString = string.Empty;

            foreach (char c in input)
            {
                if (char.IsDigit(c))
                {
                    numberString += c;
                }
            }

            if (!string.IsNullOrEmpty(numberString))
            {
                return int.Parse(numberString);
            }
            else
            {
                throw new ArgumentException("The input string does not contain a valid integer value.");
            }
        }

        private int DetectFormatType(string input)
        {
            // Check the format and return the corresponding integer
            if (input.StartsWith("¥"))
            {
                return 0; // ¥ format
            }
            else if (input.EndsWith("BP"))
            {
                return 1; // BP format
            }
            else if (input.EndsWith("PM"))
            {
                return 2; // PM format
            }
            else
            {
                throw new ArgumentException("The input string does not match any known format.");
            }
        }

        private void ReturnToShop(object sender, RoutedEventArgs e)
        {
            //Play the clicked button sound effect.
            buttonSound.Play();
            buttonSound.Position = TimeSpan.Zero;

            buyPanel.Visibility = Visibility.Collapsed;
            buyBorder.BorderThickness = new Thickness(0);
            shopBuyViewbox.PreviewKeyDown -= Confirmation_KeyDown;
            shopBuyViewbox.PreviewKeyDown += ShopViewbox_KeyDown;
        }
    }
}
