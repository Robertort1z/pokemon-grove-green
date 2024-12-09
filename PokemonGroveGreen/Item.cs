using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    public abstract class Item
    {
        //Author: Roberto Ortiz Perez
        protected string name;
        protected string sprite;
        protected string description;

        protected int pokeDollarsCost;
        protected int battlePointsCost;
        protected int pokeMilesCost;

        protected int pokeDollarsSellPrice;
        protected int battlePointsSellPrice;
        protected int pokeMilesSellPrice;

        public abstract void Use(Pokémon pokemon);

        public string GetName() { return name; }

        public string GetSprite() { return sprite; }

        public string GetDescription() { return description; }

        public int GetCost()
        {
            if (pokeDollarsCost != 0)
            {
                return pokeDollarsCost;
            }
            else if (battlePointsCost != 0)
            {
                return battlePointsCost;
            }
            else
            {
                return pokeMilesCost;
            }
        }

        public string GetCostString()
        {
            if (pokeDollarsCost != 0)
            {
                return "¥" + pokeDollarsCost;
            }
            else if (battlePointsCost != 0)
            {
                return battlePointsCost + " BP";
            }
            else
            {
                return pokeMilesCost + " PM";
            }
        }

        public int GetSellPrice()
        {
            if (pokeDollarsSellPrice != 0)
            {
                return pokeDollarsSellPrice;
            }
            else if (battlePointsSellPrice != 0)
            {
                return battlePointsSellPrice;
            }
            else
            {
                return pokeMilesSellPrice;
            }
        }

        public string GetSellPriceString()
        {
            if (pokeDollarsSellPrice != 0)
            {
                return "¥" + pokeDollarsSellPrice;
            }
            else if (battlePointsSellPrice != 0)
            {
                return battlePointsSellPrice + " BP";
            }
            else
            {
                return pokeMilesSellPrice + " PM";
            }
        }

        public override string ToString()
        {
            string cost;
            if (pokeDollarsCost != 0)
                cost = "Cost: ¥" + pokeDollarsCost.ToString();
            else if (battlePointsCost != 0)
                cost = "Cost: " + battlePointsCost.ToString() + " BP";
            else
                cost = "Cost: " + pokeMilesCost.ToString() + " PM";
            
            return cost + "\n\n" + description;
        }

        public void ThrowAway()
        {
            //Tells the list to make this object null.
        }
    }
}
