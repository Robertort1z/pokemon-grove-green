using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PokemonGroveGreen
{
    [Serializable]
    public class EvolutionStone : Item
    {
        //Author: Roberto Ortiz Perez
        MyList<(int specieId, int evolutionId)> evolutions;

        public EvolutionStone(int itemId)
        {
            evolutions = new MyList<(int specieId, int evolutionId)>();
            LoadData(itemId);
        }

        public override void Use(Pokémon pokemon)
        {
            throw new NotImplementedException();
        }

        public void Use(Random r, Pokémon pokemon)
        {
            foreach (var evolution in evolutions)
            {
                if (evolution.specieId == pokemon.GetSpecie().GetSpeciesNumber())
                {
                    pokemon.Evolve(r, evolution.evolutionId);
                }
            }
        }

        private void LoadData(int itemId)
        {
            using (var connection = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                connection.Open();

                string queryItem = "SELECT * FROM Item WHERE id = @itemId";
                using (var cmd = new MySqlCommand(queryItem, connection))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    using (var reader = cmd.ExecuteReader())
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
                        }
                    }
                }

                string queryEvolutions = @"
                    SELECT specie_id, evolution_id 
                    FROM EvolutionSpecie 
                    WHERE evolution_stone_id = @itemId";

                using (var cmd = new MySqlCommand(queryEvolutions, connection))
                {
                    cmd.Parameters.AddWithValue("@itemId", itemId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int specieId = reader.GetInt32("specie_id");
                            int evolutionId = reader.GetInt32("evolution_id");
                            evolutions.Add((specieId, evolutionId));
                        }
                    }
                }
            }
        }
    }
}
