using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    public class Pokedoll : Item
    {
        //Author: Roberto Ortiz Perez
        public Pokedoll()
        {
            this.name = "Pokedoll";
            this.sprite = "/Images/pokedoll.png";
            this.description = "A doll that attracts Pokémon. Use it to flee from any battle with a wild Pokémon.";

            this.pokeDollarsCost = 300;
            this.battlePointsCost = 0;
            this.pokeMilesCost = 0;

            this.pokeDollarsSellPrice = 75;
            this.battlePointsSellPrice = 0;
            this.pokeMilesSellPrice = 0;
        }

        public override void Use(Pokémon pokemon)
        {
            //If this is used we run away of the battle.
            this.ThrowAway();
        }
    }
}
