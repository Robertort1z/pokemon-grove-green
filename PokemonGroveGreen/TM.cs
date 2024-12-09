using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    [Serializable]
    public class TM : Item
    {
        //Author: Roberto Ortiz Perez
        Move move;

        public TM(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Item i
                    JOIN 
                        Tmshms p ON i.id = p.item_id
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
                            this.move = new Move(reader.GetInt32("move_number"));
                        }
                    }
                }
            }
        }

        public override void Use(Pokémon pokemon)
        {
            throw new NotImplementedException("TM class does not use this method.");
        }

        public void Use(Pokémon pokemon, int replacedMoveIndex)
        {
            pokemon.ReplaceMove(move, replacedMoveIndex);
        }
    }
}
