using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    public class TrainerBag
    {
        //Author: Roberto Ortiz Perez
        //An array of 8 lists of BagItems.
        MyList<BagItem>[] bag;

        public TrainerBag()
        {
            bag = new MyList<BagItem>[8];
            for (int i = 0; i < bag.Length; i += 1)
            {
                bag[i] = new MyList<BagItem>();
            }
        }

        internal MyList<BagItem>[] GetBag() { return bag; }

        public bool Add(int pocketIndex, Item item, int quantity)
        {
            try
            {
                //First check we dont have said item.
                foreach (var bagItem in bag[pocketIndex])
                {
                    //We compare their names.
                    if (bagItem.GetItem().GetName() == item.GetName())
                    {
                        //We already have that item, so we add quantity.
                        bagItem.Add(quantity);
                        return true;
                    }
                }

                //We don't have said item, so we add it to the list.
                BagItem newItems = new BagItem(item, quantity);
                bag[pocketIndex].Add(newItems);
                return true;

            } catch (Exception)
            {
                return false;
            }
        }

        public bool Remove(int pocketIndex, Item item, int quantity)
        {
            try
            {
                //First check we have said item.
                foreach (var bagItem in bag[pocketIndex])
                {
                    //We compare their names.
                    if (bagItem.GetItem().GetName() == item.GetName())
                    {
                        //We have that item, so we remove quantity or the item.
                        bagItem.Remove(quantity);
                        if (bagItem.GetQuantity() <= 0)
                            bag[pocketIndex].Remove(bagItem);
                        return true;
                    }
                }
                //We have not found the item.
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        internal BagItem GetElement(int pocketIndex, int index)
        {
            return bag[pocketIndex].GetElem(index);
        }
    }
}
