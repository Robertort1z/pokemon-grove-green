using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    public class Berry : Item
    {
        //Author: Roberto Ortiz Perez
        int healPower;
        int revivePower;
        int etherPower;

        public Berry(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Item i
                    JOIN 
                        Berry p ON i.id = p.item_id
                    WHERE 
                        i.id = @itemId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.name = reader.GetString("name_");
                            this.sprite = reader.GetString("sprite");
                            this.description = reader.GetString("description_");
                            this.pokeDollarsCost = reader.GetInt32("pokeDollarsCost");
                            this.battlePointsCost = reader.GetInt32("battlePointsCost");
                            this.pokeMilesCost = reader.GetInt32("pokeMilesCost");
                            this.pokeDollarsSellPrice = reader.GetInt32("pokeDollarsSellPrice");
                            this.battlePointsSellPrice = reader.GetInt32("battlePointsSellPrice");
                            this.pokeMilesSellPrice = reader.GetInt32("pokeMilesSellPrice");
                            this.healPower = reader.GetInt32("heal_power");
                            this.revivePower = reader.GetInt32("revive_power");
                            this.etherPower = reader.GetInt32("ether_power");
                        }
                    }
                }
            }
        }

        public override void Use(Pokémon pokemon)
        {
            if (healPower != 0)
                pokemon.Heal(healPower);
            else if (revivePower != 0)
                pokemon.Revive(revivePower);
            else
                pokemon.RestoreAllPP(etherPower);
            this.ThrowAway();
        }
    }
}
