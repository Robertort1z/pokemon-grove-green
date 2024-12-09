using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonGroveGreen
{
    public enum Kind
    {
        PHYSICAL,
        SPECIAL,
        NULL
    }

    [Serializable]
    public class Move
    {
        //Author: Roberto Ortiz Perez
        int moveId;
        string name;
        string description;

        int powerPoints;
        int currentPowerPoints;
        int power;
        int accuracy;        
        int priority;

        Type type;
        Kind kind;

        public Move(int moveId)
        {
            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=SpeciesMovesDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Moves
                    WHERE 
                        moveId = @moveId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@moveId", moveId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.moveId = moveId;
                            this.name = reader.GetString("name_");
                            this.description = reader.GetString("description_");
                            this.powerPoints = reader.GetInt32("power_points");
                            this.currentPowerPoints = powerPoints;
                            this.power = reader.GetInt32("power");
                            this.accuracy = reader.GetInt32("accuracy");
                            this.priority = reader.GetInt32("priority");
                            this.type = (Type)Enum.Parse(typeof(Type), reader.GetString("type_"));
                            this.kind = (Kind)Enum.Parse(typeof(Kind), reader.GetString("kind"));
                        }
                    }
                }
            }
        }

        public int GetMoveId() { return moveId; }

        public string GetName() { return name; }

        public string GetDescription() { return description; }

        public int GetPowerPoints() { return powerPoints; }

        public int GetCurrentPowerPoints() { return currentPowerPoints; }

        public int GetPower() { return power; }

        public int GetAccuracy() { return accuracy; }

        public int GetPriority() { return priority; }

        public Kind GetKind() { return kind; }

        public Type GetMoveType() { return type; }

        // ---------- Everything above are getters or setters. ----------

        public void RestoreAllPowerPoints() { currentPowerPoints = powerPoints; }

        public void RestorePowerPoints(int amount) 
        { 
            currentPowerPoints += amount;
            if (currentPowerPoints > powerPoints)
                currentPowerPoints = powerPoints;
        }

        public void UsePP() { currentPowerPoints -= 1; }
    }
}
