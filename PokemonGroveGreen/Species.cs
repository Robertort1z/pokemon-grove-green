using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using Org.BouncyCastle.Asn1.Tsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace PokemonGroveGreen
{
    public enum Type
    {
        NORMAL,
        FIRE,
        WATER,
        GRASS,
        ELECTRIC,
        ICE,
        FIGHTING,
        POISON,
        GROUND,
        FLYING,
        PSYCHIC,
        BUG,
        ROCK,
        GHOST,
        DRAGON,
        DARK,
        STEEL,
        FAIRY,
        NULL
    }


    [Serializable]
    public class Species
    {
        //Author: Roberto Ortiz Perez
        //Readonly makes the array to only be created once, independently of the 'species' objects that I make.
        static readonly float[][] Compatibility = new float[][]
        {
            new float[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0.5f, 0, 1, 1, 0.5f, 1, 1},
            new float[] {1, 0.5f, 0.5f, 2, 1, 2, 1, 1, 1, 1, 1, 2, 0.5f, 1, 0.5f, 1, 2, 1, 1},
            new float[] {1, 2, 0.5f, 0.5f, 1, 1, 1, 1, 2, 1, 1, 1, 2, 1, 0.5f, 1, 1, 1, 1},
            new float[] {1, 0.5f, 2, 0.5f, 1, 1, 1, 0.5f, 2, 0.5f, 1, 0.5f, 2, 1, 0.5f, 1, 0.5f, 1, 1},
            new float[] {1, 1, 2, 0.5f, 0.5f, 1, 1, 1, 0, 2, 1, 1, 1, 1, 0.5f, 1, 1, 1, 1},
            new float[] {1, 0.5f, 0.5f, 2, 1, 0.5f, 1, 1, 2, 2, 1, 1, 1, 1, 2, 1, 0.5f, 1, 1},
            new float[] {2, 1, 1, 1, 1, 2, 1, 0.5f, 1, 0.5f, 0.5f, 0.5f, 2, 0, 1, 2, 2, 0.5f, 1},
            new float[] {1, 1, 1, 2, 1, 1, 1, 0.5f, 0.5f, 1, 1, 1, 0.5f, 0.5f, 1, 1, 0, 2, 1},
            new float[] {1, 2, 1, 0.5f, 2, 1, 1, 2, 1, 0, 1, 0.5f, 2, 1, 1, 1, 2, 1, 1},
            new float[] {1, 1, 1, 2, 0.5f, 1, 2, 1, 1, 1, 1, 2, 0.5f, 1, 1, 1, 0.5f, 1, 1},
            new float[] {1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 0.5f, 1, 1, 1, 1, 0, 0.5f, 1, 1},
            new float[] {1, 0.5f, 1, 2, 1, 1, 0.5f, 0.5f, 1, 0.5f, 2, 1, 1, 0.5f, 1, 2, 0.5f, 0.5f, 1},
            new float[] {1, 2, 1, 1, 1, 2, 0.5f, 1, 0.5f, 2, 1, 2, 1, 1, 1, 1, 0.5f, 1, 1},
            new float[] {0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 2, 1, 0.5f, 1, 1, 1},
            new float[] {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 0.5f, 0, 1},
            new float[] {1, 1, 1, 1, 1, 1, 0.5f, 1, 1, 1, 2, 1, 1, 2, 1, 0.5f, 1, 0.5f, 1},
            new float[] {1, 0.5f, 0.5f, 1, 0.5f, 2, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 0.5f, 2, 1},
            new float[] {1, 0.5f, 1, 1, 1, 1, 2, 0.5f, 1, 1, 1, 1, 1, 1, 2, 2, 0.5f, 1, 1}
        };

        int speciesNumber;
        string speciesName;
        string speciesTitle;
        int captureRate; //Chances to be captured. Between 0 and 255.
        int genderRate; //Chances to be male. Between 0 and 100. -1 is equal to genderless.

        Type firstType;
        Type secondType;

        MyList<Move> movePool; //Every species of pokemon has 4 moves.

        //Between 0 and 255.
        int hp;
        int attack;
        int defense;
        int spAtk;
        int spDef;
        int speed;

        int rewardRate; //Amount of money the pokémon gives after defeat.

        string frontSprite;
        string backSprite;

        public Species(Random r, int speciesNumber)
        {
            movePool = new MyList<Move>();

            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=SpeciesMovesDB;user=root;password=root"))
            {
                conn.Open();

                string query = @"
                    SELECT *
                    FROM 
                        Species
                    WHERE 
                        specieid = @specieId";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@specieId", speciesNumber);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.speciesNumber = speciesNumber;
                            this.speciesName = reader.GetString("name_");
                            this.speciesTitle = reader.GetString("title");
                            this.captureRate = reader.GetInt32("captureRate");
                            this.genderRate = reader.GetInt32("genderRate");
                            this.firstType = (Type)Enum.Parse(typeof(Type), reader.GetString("firstType"));
                            this.secondType = (Type)Enum.Parse(typeof(Type), reader.GetString("secondType"));
                            this.hp = (int)Math.Round(reader.GetInt32("hp") * 2 * (0.9 + r.NextDouble() * 0.2));
                            this.attack = (int)Math.Round(reader.GetInt32("attack") * (0.9 + r.NextDouble() * 0.2));
                            this.defense = (int)Math.Round(reader.GetInt32("defense") * (0.9 + r.NextDouble() * 0.2));
                            this.spAtk = (int)Math.Round(reader.GetInt32("spAtk") * (0.9 + r.NextDouble() * 0.2));
                            this.spDef = (int)Math.Round(reader.GetInt32("spDef") * (0.9 + r.NextDouble() * 0.2));
                            this.speed = (int)Math.Round(reader.GetInt32("speed") * (0.9 + r.NextDouble() * 0.2));
                            this.rewardRate = (hp + attack + defense + spAtk + spDef + speed) * 2; 
                            this.frontSprite = reader.GetString("frontSprite");
                            this.backSprite = reader.GetString("backSprite");
                        }
                    }
                }

                // Load move pool
                string queryMoves = @"
                SELECT 
                    moveId
                FROM 
                    Knows
                WHERE 
                    specieId = @specieId
                ORDER BY 
                    RAND()";

                using (MySqlCommand cmd = new MySqlCommand(queryMoves, conn))
                {
                    cmd.Parameters.AddWithValue("@specieId", speciesNumber);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int moveId = reader.GetInt32("moveId");
                            Move move = new Move(moveId);
                            movePool.Add(move);
                        }
                    }
                }
            }
        }

        public Species(Random r)
        {
            movePool = new MyList<Move>();

            using (MySqlConnection conn = new MySqlConnection("server=localhost;database=SpeciesMovesDB;user=root;password=root"))
            {
                conn.Open();

                string query = "SELECT * FROM Species ORDER BY RAND() LIMIT 1";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            this.speciesNumber = reader.GetInt32("specieId");
                            this.speciesName = reader.GetString("name_");
                            this.speciesTitle = reader.GetString("title");
                            this.captureRate = reader.GetInt32("captureRate");
                            this.genderRate = reader.GetInt32("genderRate");
                            this.firstType = (Type)Enum.Parse(typeof(Type), reader.GetString("firstType"));
                            this.secondType = (Type)Enum.Parse(typeof(Type), reader.GetString("secondType"));
                            this.hp = (int)Math.Round(reader.GetInt32("hp") * 2 * (0.9 + r.NextDouble() * 0.2));
                            this.attack = (int)Math.Round(reader.GetInt32("attack") * (0.9 + r.NextDouble() * 0.2));
                            this.defense = (int)Math.Round(reader.GetInt32("defense") * (0.9 + r.NextDouble() * 0.2));
                            this.spAtk = (int)Math.Round(reader.GetInt32("spAtk") * (0.9 + r.NextDouble() * 0.2));
                            this.spDef = (int)Math.Round(reader.GetInt32("spDef") * (0.9 + r.NextDouble() * 0.2));
                            this.speed = (int)Math.Round(reader.GetInt32("speed") * (0.9 + r.NextDouble() * 0.2));
                            this.rewardRate = (hp + attack + defense + spAtk + spDef + speed) * 2;
                            this.frontSprite = reader.GetString("frontSprite");
                            this.backSprite = reader.GetString("backSprite");
                        }
                    }
                }

                // Load move pool
                string queryMoves = @"
                SELECT 
                    moveId
                FROM 
                    Knows
                WHERE 
                    specieId = @specieId";

                using (MySqlCommand cmd = new MySqlCommand(queryMoves, conn))
                {
                    cmd.Parameters.AddWithValue("@specieId", speciesNumber);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int moveId = reader.GetInt32("moveId");
                            Move move = new Move(moveId);
                            movePool.Add(move);
                        }
                    }
                }
            }
        }

        public int GetSpeciesNumber() { return speciesNumber; }

        public string GetSpeciesName() { return speciesName; }

        public int GetCaptureRate() { return captureRate; }

        public int GetGenderRate() { return genderRate; }

        public string GetTitle() { return speciesTitle; }

        public Type GetFirstType() { return firstType; }

        public Type GetSecondType() { return secondType; }

        public MyList<Move> GetMovePool() { return movePool; }

        public int GetHP() { return hp; }

        public int GetAttack() { return attack; }

        public int GetDefense() { return defense; }

        public int GetSpAtk() { return spAtk; }

        public int GetSpDef() { return spDef; }

        public int GetSpeed() { return speed; }

        public int GetRewardRate() { return rewardRate; }

        public string GetFrontSprite() { return frontSprite; }

        public string GetBackSprite() { return backSprite; }

        //Everything above are getters or setters.

        public float GetCompatibility(Type attacker, Type defender)
        {
            return Compatibility[(int)attacker][(int)defender];
        }

        public override string ToString()
        {
            return "Species Number: " + speciesNumber +
                "\nSpecies Name: " + speciesName +
                "\nSpecies Title: " + speciesTitle +
                "\nFirst Type: " + firstType.ToString() +
                "\nSecond Type: " + secondType.ToString() +
                "\nHP: " + hp +
                "\nAttack: " + attack +
                "\nDefense: " + defense +
                "\nSp. Atk: " + spAtk +
                "\nSp. Def: " + spDef +
                "\nSpeed: " + speed;
        }
    }
}
