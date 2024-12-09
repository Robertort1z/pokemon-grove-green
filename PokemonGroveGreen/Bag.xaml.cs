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
    /// Lógica de interacción para Bag.xaml
    /// </summary>
    public partial class Bag : Page
    {
        //Author: Roberto Ortiz Perez
        Trainer trainer;
        TrainerBag trainerBag;
        MyList<Item> currentPocket;
        Item item;
        BagItem bagItem;

        int pocketIndex = 0;
        int selectedItem = 0;
        int quantity = 0;
        int maxQuantity;
        bool saleable;
        bool combat;
        bool areEventsEnabled = true;
        int price;

        public Bag(Trainer trainer, bool saleable, bool combat)
        {
            InitializeComponent();
            mainViewbox.Focus();

            this.trainer = trainer;
            this.trainerBag = trainer.GetBag();

            this.saleable = saleable;
            this.combat = combat;

            item = new Potion(1);
            bagItem = new BagItem(item, 0);

            DisplayItems();
        }

        private void Viewbox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D:
                    {
                        MoveRight();  
                        break;
                    }
                case Key.A:
                    {
                        MoveLeft();
                        break;
                    }
                case Key.Escape:
                    {
                        this.NavigationService.GoBack();
                        break;
                    }
            }
        }

        private void Viewbox_KeyDownQuantity(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    {
                        if (quantity == maxQuantity)
                            break;
                        quantity += 1;
                        quantityBlock.Text = "x" + quantity.ToString("D3");
                        quantityPrice.Text = MultiplyPrice(item, quantity);
                        break;
                    }
                case Key.S:
                    {
                        if (quantity == 1)
                            break;
                        quantity -= 1;
                        quantityBlock.Text = "x" + quantity.ToString("D3");
                        quantityPrice.Text = MultiplyPrice(item, quantity);
                        break;
                    }
                case Key.Enter:
                    {
                        ConfirmSale();
                        break;
                    }
                case Key.Escape:
                    {
                        ReturnToBag();
                        break;
                    }
            }
        }

        private void Viewbox_KeyDownMakeSale(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        MakeSale();
                        break;
                    }
                case Key.Escape:
                    {
                        ReturnToBag();
                        break;
                    }
            }
        }

        private async void MakeSale()
        {
            confirmationBlock.Text = "Thank you! Please come back again!";

            //Remove the item.
            trainerBag.Remove(pocketIndex, item, quantity);

            if (quantityPrice.Text.StartsWith("¥"))
            {
                //PokeDollars
                trainer.GetWallet().AddDollars(price);               
            }
            else if (quantityPrice.Text.EndsWith(" BP"))
            {
                //BattlePoints
                trainer.GetWallet().AddBattlePoints(price);
            }
            else if (quantityPrice.Text.EndsWith(" PM"))
            {
                //PokeMiles
                trainer.GetWallet().AddPokeMiles(price);
            }

            //Update the wallet display.
            walletBlock.Text = "Wallet\n\t" + "¥" + trainer.GetPokeDollars() + "\n\t" + trainer.GetBattlePoints()
                + " BP\n\t" + trainer.GetPokeMiles() + " PM";

            //Update our items.
            ClearItemList();
            DisplayItems();

            mainViewbox.PreviewKeyDown -= Viewbox_KeyDownMakeSale;

            await Task.Delay(1000);

            ReturnToBag();
        }

        private void ConfirmSale()
        {
            //Show confirmation screen.
            confirmationBlock.Text = "I can pay " + quantityPrice.Text + ".\n" + "Would that be okay?";
            itemQuantityBorder.Visibility = Visibility.Collapsed;
            confirmationBorder.Visibility = Visibility.Visible;

            //Update events.
            mainViewbox.PreviewKeyDown -= Viewbox_KeyDownQuantity;
            mainViewbox.PreviewKeyDown += Viewbox_KeyDownMakeSale;
        }

        private void ReturnToBag()
        {
            mainViewbox.PreviewKeyDown -= Viewbox_KeyDownQuantity;
            mainViewbox.PreviewKeyDown -= Viewbox_KeyDownMakeSale;
            mainViewbox.PreviewKeyDown += Viewbox_KeyDown;
            areEventsEnabled = true;
            walletBorder.Visibility = Visibility.Collapsed;
            itemQuantityBorder.Visibility = Visibility.Collapsed;
            confirmationBorder.Visibility = Visibility.Collapsed;
            itemImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));
            itemDescription.Text = string.Empty;
        }

        private void MoveLeft()
        {
            //Update currentPocket
            if (pocketIndex == 0)
                pocketIndex = 7;
            else
                pocketIndex -= 1;

            //Delete previous items.
            ClearItemList();

            //We act different depending on which pocket we currently are.
            switch (pocketBlock.Text)
            {
                case "Items":
                    {
                        itemsIcon.BorderThickness = new Thickness(0);
                        keyItemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Key Items";
                        break;
                    }
                case "Medicine":
                    {
                        medicineIcon.BorderThickness = new Thickness(0);
                        itemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Items";
                        break;
                    }
                case "Poké Balls":
                    {
                        pokéBallsIcon.BorderThickness = new Thickness(0);
                        medicineIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Medicine";
                        break;
                    }
                case "TMs & HMs":
                    {
                        tmshmsIcon.BorderThickness = new Thickness(0);
                        pokéBallsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Poké Balls";
                        break;
                    }
                case "Berries":
                    {
                        berriesIcon.BorderThickness = new Thickness(0);
                        tmshmsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "TMs & HMs";
                        break;
                    }
                case "Treasures":
                    {
                        treasuresIcon.BorderThickness = new Thickness(0);
                        berriesIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Berries";
                        break;
                    }
                case "Battle Items":
                    {
                        battleItemsIcon.BorderThickness = new Thickness(0);
                        treasuresIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Treasures";
                        break;
                    }
                case "Key Items":
                    {
                        keyItemsIcon.BorderThickness = new Thickness(0);
                        battleItemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Battle Items";
                        break;
                    }
            }

            DisplayItems();
        }

        private void MoveRight()
        {
            //Update currentPocket
            if (pocketIndex == 7)
                pocketIndex = 0;
            else
                pocketIndex += 1;

            //Delete previous items.
            ClearItemList();

            //We act different depending on which pocket we currently are.
            switch (pocketBlock.Text)
            {
                case "Items":
                    {
                        itemsIcon.BorderThickness = new Thickness(0);
                        medicineIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Medicine";                       
                        break;
                    }
                case "Medicine":
                    {
                        medicineIcon.BorderThickness = new Thickness(0);
                        pokéBallsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Poké Balls";
                        break;
                    }
                case "Poké Balls":
                    {
                        pokéBallsIcon.BorderThickness = new Thickness(0);
                        tmshmsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "TMs & HMs";
                        break;
                    }
                case "TMs & HMs":
                    {
                        tmshmsIcon.BorderThickness = new Thickness(0);
                        berriesIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Berries";
                        break;
                    }
                case "Berries":
                    {
                        berriesIcon.BorderThickness = new Thickness(0);
                        treasuresIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Treasures";
                        break;
                    }
                case "Treasures":
                    {
                        treasuresIcon.BorderThickness = new Thickness(0);
                        battleItemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Battle Items";
                        break;
                    }
                case "Battle Items":
                    {
                        battleItemsIcon.BorderThickness = new Thickness(0);
                        keyItemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Key Items";
                        break;
                    }
                case "Key Items":
                    {
                        keyItemsIcon.BorderThickness = new Thickness(0);
                        itemsIcon.BorderThickness = new Thickness(3);
                        pocketBlock.Text = "Items";
                        break;
                    }
            }

            DisplayItems();
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            if (areEventsEnabled)
            {
                // Get the position of the item
                int position = itemList.Items.IndexOf((sender as ListBoxItem).Content);

                //Get the item.
                Item item = trainerBag.GetElement(pocketIndex, position).GetItem();

                //Update image.
                string itemSprite = item.GetSprite();
                itemImage.Source = new BitmapImage(new Uri(itemSprite, UriKind.Relative));

                //Update description.
                itemDescription.Text = item.GetDescription();
            }
        }

        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            if (areEventsEnabled)
            {
                //Delete the image.
                itemImage.Source = new BitmapImage(new Uri(string.Empty, UriKind.Relative));

                //Delete the description.
                itemDescription.Text = string.Empty;
            }
        }

        private void ListBoxItem_PreviewDown(object sender, MouseButtonEventArgs e)
        {
            if (areEventsEnabled)
            {
                // Get the position of the item
                int position = itemList.Items.IndexOf((sender as ListBoxItem).Content);

                //Get the item.
                bagItem = trainerBag.GetElement(pocketIndex, position);
                item = bagItem.GetItem();

                //Disable changing pockets and other events.
                mainViewbox.PreviewKeyDown -= Viewbox_KeyDown;
                mainViewbox.PreviewKeyDown += Viewbox_KeyDownQuantity;
                areEventsEnabled = false;

                if (saleable == true)
                {
                    quantity = 1;
                    maxQuantity = bagItem.GetQuantity();
                    SelectSelling(item);
                }
                else if (combat == true)
                {
                    //Use some items.
                }
                else
                {
                    //Use some items.
                }
            }
        }

        private void SelectSelling(Item item)
        {
            itemDescription.Text = item.GetName() + "?\n" + "How many would you like to sell?";
            
            //Show wallet block
            walletBorder.Visibility = Visibility.Visible;
            walletBlock.Text = "Wallet\n\t" + "¥" + trainer.GetPokeDollars() + "\n\t" + trainer.GetBattlePoints() 
                + " BP\n\t" + trainer.GetPokeMiles() + " PM";

            //Show quantity selection.
            itemQuantityBorder.Visibility = Visibility.Visible;
            quantityBlock.Text = "x" + quantity.ToString("D3");
            quantityPrice.Text = MultiplyPrice(item, quantity);
        }

        private void DisplayItems()
        {
            foreach (var item in trainerBag.GetBag()[pocketIndex])
            {
                itemList.Items.Add(new ItemInfo(item.GetName(), "x" + item.GetQuantity()));
            }
        }

        private void ClearItemList()
        {
            itemList.Items.Clear();
        }

        public string MultiplyPrice(Item item, int multiplier)
        {
            string priceString = item.GetSellPriceString();
            price = item.GetSellPrice() * multiplier;
            //Checks the format.
            if (priceString.StartsWith("¥"))
            {
                //PokeDollars
                return "¥" + price;
            }
            else if (priceString.EndsWith(" BP"))
            {
                //BattlePoints
                return price + " BP";
            }
            else if (priceString.EndsWith(" PM"))
            {
                //PokeMiles
                return price + " PM";
            }

            //If the format does not match with any of the expected ones.
            throw new ArgumentException("The price format is not valid.", nameof(priceString));
        }
    }

    public class ItemInfo
    {
        public string LeftPart { get; set; }
        public string RightPart { get; set; }

        public ItemInfo(string leftPart, string rightPart)
        {
            LeftPart = leftPart;
            RightPart = rightPart;
        }

        public override string ToString()
        {
            return $"{LeftPart} {RightPart}";
        }
    }
}
