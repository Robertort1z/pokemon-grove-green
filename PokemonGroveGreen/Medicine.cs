using System;
using MySql;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using System.Security.Policy;
using System.Xml.Linq;
using Google.Protobuf.WellKnownTypes;

namespace PokemonGroveGreen
{
    [Serializable]
    public abstract class Medicine : Item
    {
        //Author: Roberto Ortiz Perez
        public abstract override void Use(Pokémon pokemon);
    }

    [Serializable]
    public class Potion : Medicine
    {
        int healPower;

        public Potion(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Item i
                    JOIN 
                        Potion p ON i.id = p.item_id
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
                        }
                    }
                }
            }
        }

        public override void Use(Pokémon pokemon) 
        { 
            pokemon.Heal(healPower); 
            this.ThrowAway();
        }
    }

    [Serializable]
    public class Revive : Medicine
    {
        int revivePower;

        public Revive(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Item i
                    JOIN 
                        Revive p ON i.id = p.item_id
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
                            this.revivePower = reader.GetInt32("revive_power");
                        }
                    }
                }
            }
        }

        public override void Use(Pokémon pokemon) 
        { 
            pokemon.Revive(revivePower);
            this.ThrowAway();
        }
    }

    [Serializable]
    public class Ether : Medicine
    {
        int etherPower;
        int etherExtension;

        public Ether(int itemId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=ItemsDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Item i
                    JOIN 
                        Ether p ON i.id = p.item_id
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
                            this.etherPower = reader.GetInt32("ether_power");
                            this.etherExtension = reader.GetInt32("ether_extension");
                        }
                    }
                }
            }
        }

        public override void Use(Pokémon pokemon) 
        { 
            pokemon.RestoreAllPP(etherPower);
            this.ThrowAway();
        }

        public void Use(Pokémon pokemon, int moveIndex) 
        { 
            pokemon.RestorePP(etherPower, pokemon.GetMove(moveIndex));
            this.ThrowAway();
        }
    }
}
