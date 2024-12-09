using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonGroveGreen
{
    public enum Gender 
    { 
        MALE, 
        FEMALE,
        GENDERLESS
    }

    [Serializable]
    public class Pokémon
    {
        //Author: Roberto Ortiz Perez
        string nickname;
        Gender gender;
        DateTime? captureDate; //Allows the DateTime to be set to null in case it is a wild Pokémon.
        Identity eo;
        Move[] moves;
        int currentHP;
        Species specie;

        public Pokémon(Random r, int speciesNumber, string nickname)
        {
            specie = new Species(r, speciesNumber);
            this.nickname = nickname;
            this.gender = GenerateGender(r);
            this.captureDate = null;
            eo = null;
            moves = new Move[4];
            GetRandomMoves();
            currentHP = specie.GetHP();
        }

        public Pokémon(Random r, int speciesNumber)
        {
            specie = new Species(r, speciesNumber);
            this.nickname = specie.GetSpeciesName();
            this.gender = GenerateGender(r);
            this.captureDate = null;
            eo = null;
            moves = new Move[4];
            GetRandomMoves();
            currentHP = specie.GetHP();
        }

        public Pokémon(Random r)
        {
            specie = new Species(r);
            this.nickname = specie.GetSpeciesName();
            this.gender = GenerateGender(r);
            this.captureDate = null;
            eo = null;
            moves = new Move[4];
            GetRandomMoves();
            currentHP = specie.GetHP();
        }

        public Gender GetGender() { return this.gender; }

        public int GetCurrentHP() { return this.currentHP; }

        public Species GetSpecie() { return this.specie; }

        public string GetNickname() { return this.nickname; } 

        public string GetTitle() { return specie.GetTitle(); }

        public string GetFrontSprite() { return specie.GetFrontSprite(); }

        public string GetBackSprite() { return specie.GetBackSprite(); }

        public int GetSpeciesNumber() { return specie.GetSpeciesNumber(); }

        public Type GetFirstType() { return specie.GetFirstType(); }

        public Type GetSecondType() { return specie.GetSecondType(); }

        public int GetHP() { return specie.GetHP(); }

        public int GetAttack() { return specie.GetAttack(); }

        public int GetDefense() { return specie.GetDefense(); }

        public int GetSpAtk() { return specie.GetSpAtk(); }

        public int GetSpDef() { return specie.GetSpDef(); }

        public int GetSpeed() { return specie.GetSpeed(); }

        public int GetRewardRate() { return specie.GetRewardRate(); }

        public string GetMoveName(int index) 
        {
            //Return the name of a move.
            if (moves[index].GetName() == null)
                return string.Empty;
            else
                return moves[index].GetName();
        }

        public Type GetMoveType(int index) { return moves[index].GetMoveType(); }

        public Move GetMove(int index) { return moves[index]; }

        public Identity GetEO() { return eo; }

        public void SetEO(Identity eo) { this.eo = eo; }

        //Everyhing above this are getters or setters.

        public Move SelectRandomMove(Random random)
        {
            //Return a random move from the ones the pokémon has available.

            Move selectedMove;
            do
            {
                // Select a random move from the available moves
                selectedMove = moves[random.Next(0, 4)];
            } while (selectedMove.GetMoveId() == -1 || selectedMove.GetCurrentPowerPoints() == 0);

            return selectedMove;
        }

        public int Attack(Random r, Pokémon enemy, Move move)
        {
            //We attack an enemy Pokémon. Returns the damage we deal to the enemy.
            //First we try the accuracy of the move.
            if (r.Next(1, 101) > move.GetAccuracy())
            {
                //The move misses.
                //We lose a PP.
                move.UsePP();
                return -1;
            }

            //We need to know whether the move is physical or special.
            int attack;
            int defense;

            if (move.GetKind() == Kind.PHYSICAL)
            {
                attack = specie.GetAttack();
                defense = enemy.specie.GetDefense();
            }
            else
            {
                attack = specie.GetSpAtk();
                defense = enemy.specie.GetSpDef();
            }

            //Check STAB.
            double stab;
            if (specie.GetFirstType() == move.GetMoveType() || specie.GetSecondType() == move.GetMoveType())
                stab = 1.5;
            else
                stab = 1;

            //Generates a random between 217 and 255, both included. Divides by 255.
            double rand = r.Next(217, 256) / 255.0;

            //Generate if the attack is a critic or not.
            int critChance = r.Next(0, 256); //Generate a random number between 0 and 255.
            int critThreshold = r.Next(0, 256); //Generate a second number between 0 and 255.
            int crit;
            if (critChance < critThreshold) //Only if critChance is strictly less than the threshold, it is a critic.
                crit = 2;
            else
                crit = 1;

            //Check the effectiveness of the move against the enemy.
            Type moveType = move.GetMoveType();
            double firstEffectiveness = specie.GetCompatibility(moveType, enemy.specie.GetFirstType());
            double secondEffectiveness = specie.GetCompatibility(moveType, enemy.specie.GetSecondType());

            //Calculate the damage dealt.
            double result = (((2 * 40 * crit / 5.0) + 2) * move.GetPower() * attack / defense / 50.0 + 2) * stab * firstEffectiveness * secondEffectiveness * rand;
            int damage = (int)Math.Round(result); // Using null-coalescing operator to handle null case

            //I don't think this can happen, but it does not hurt to check.
            if (damage < 0)
                damage = 0;

            //We deal said damage to the enemy.
            enemy.LowerHP(damage);

            //We lose a PP.
            move.UsePP();

            //We return the damage we deal to the enemy.
            return damage;
        }

        public bool Escape(Random r, Pokémon enemy, int attempts)
        {
            //We try to escape the battle. Returns true if we are successful.

            //We escape if we have higher (or the same) speed than the enemy.
            if (specie.GetSpeed() >= enemy.specie.GetSpeed())
                return true;

            //Calculate the odds we have to escape.
            int oddsEscape = ((specie.GetSpeed() * 128 / enemy.specie.GetSpeed()) + 30 * attempts) % 256;

            int rand = r.Next(0, 256);

            //We return true if we escape successfully.
            return rand < oddsEscape;
        }

        public bool Capture(Random r, int ball)
        {
            //We try to capture a Pokémon. Return true if we are successful.

            //Calculate the modified capture rate.
            int catchRate = ((3 * this.specie.GetHP() - 2 * this.currentHP) * this.specie.GetCaptureRate() * ball) / (3 * this.specie.GetHP());

            //We calculate the bump probability.
            int bump = 65536 / ((int)Math.Round(Math.Pow(255 / catchRate, 0.1875)));

            //We need to compare a random 4 times to the bump in order to catch the Pokémon.
            for (int i = 0; i < 4; i += 1)
            {
                if (r.Next(0, 65536) >= bump)
                    return false; //If the catch fails we return false.
            }
            
            //The catch has succeeded, so we return true.
            return true;
        }

        public void Evolve(Random r, int newSpecieId)
        {
            specie = new Species(r, newSpecieId);
        }

        private void LowerHP(int damage) 
        { 
            currentHP -= damage;
            
            //CurrentHP must not be lower than 0.
            if (currentHP < 0)
                currentHP = 0;
        }

        public void Heal() 
        {  
            currentHP = specie.GetHP();
            for (int i = 0; i < 4; i += 1)
            {
                moves[i].RestoreAllPowerPoints();
            }
        }

        public void Heal(int amount)
        {
            currentHP += amount;
            //We don' allow to heal over the limit.
            if (currentHP > specie.GetHP())
                currentHP = specie.GetHP();
        }

        public void Revive(int amount)
        {
            double newHealth = specie.GetHP() * amount / 100;
            this.currentHP = (int)Math.Round(newHealth);
        }

        public void RestorePP(int etherPower, Move move)
        {
            move.RestorePowerPoints(etherPower);
        }

        public void RestoreAllPP(int etherPower)
        {
            for (int i = 0; i < 4; i += 1)
            {
                moves[i].RestorePowerPoints(etherPower);
            }
        }

        public void ReplaceMove(Move newMove, int replacedMoveIndex)
        {
            moves[replacedMoveIndex] = newMove;
        }

        private Gender GenerateGender(Random r)
        {
            int randomGender = r.Next(0,101); //Up to 100, in case the gender rate is 99%.
            if (specie.GetGenderRate() == -1)
                return Gender.GENDERLESS;
            else if (randomGender > specie.GetGenderRate())
                return Gender.MALE;
            else
                return Gender.FEMALE;
        }

        public void GetRandomMoves()
        {
            Move move;
            for (int i = 0; i < 4; i += 1)
            {
                move = specie.GetMovePool().GetElem(i);
                if (move != null)
                    moves[i] = move;
                else
                    moves[i] = new Move(-1);
            }
        }

        public override string ToString()
        {
            string info;
            info = "Nickname: " + nickname +
                "\nGender: " + gender.ToString() +
                "\nCapture Date: " + captureDate.ToString() +
                "\nEO: " + eo.GetName() +
                "\nCurrent health: " + currentHP;
            foreach (Move move in moves)
            {
                if (move.GetName() != string.Empty)
                    info += "\n" + move.GetName();
            }
            info += specie.ToString();
            return "";
        }
    }
}
