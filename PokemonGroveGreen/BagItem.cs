using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    internal class BagItem
    {
        //Author: Roberto Ortiz Perez
        public Item item;
        public int quantity;

        public BagItem(Item item, int quantity)
        {
            this.item = item;
            this.quantity = quantity;
        }

        public Item GetItem() { return item; }

        public string GetName() { return item.GetName(); }

        public string GetItemSprite() { return item.GetSprite(); }

        public int GetQuantity() { return quantity; }

        public void Add(int addition) { quantity += addition; }

        public void Remove(int subtraction) { quantity -= subtraction; }

        public override string ToString()
        {
            return item.GetName() + " x" + quantity;
        }
    }
}
