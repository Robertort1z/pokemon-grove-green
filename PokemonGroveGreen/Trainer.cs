using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace PokemonGroveGreen
{
    [Serializable]
    public class Trainer
    {
        //Author: Roberto Ortiz Perez
        Identity identity;
        Wallet wallet;
        TrainerBag bag;
        
        DateTime creationTime;
        TimeSpan playTime;
        
        Pokémon[] pokemonTeam;
        Pokémon[][] pokemonVault;
        int capturedPokemon;
        string sprite;

        public Trainer(Random r, string name, Gender gender, int regionId)
        {
            if (name == "justabox")
                AdminAccess(r, name, gender, regionId);
            else
                RegularAccess(r, name, gender, regionId);
        }

        private void RegularAccess(Random r, string name, Gender gender, int regionId)
        {
            identity = new Identity(r, name, gender, regionId);

            //Creates an empty wallet.
            wallet = new Wallet();

            //Creates an empty bag.
            bag = new TrainerBag();

            creationTime = DateTime.Now;

            //Creates a TimeSpan with 0h, 0m and 0s.
            playTime = new TimeSpan(0, 0, 0);

            //Creates a new pokémon team. The size is always six.
            pokemonTeam = new Pokémon[6];

            //Creates the pokémon vault. The size is always the same.
            pokemonVault = new Pokémon[3][];
            for (int i = 0; i < 3; i += 1)
            {
                pokemonVault[i] = new Pokémon[32];
            }

            capturedPokemon = 0;

            sprite = GenerateSprite();
        }

        private void AdminAccess(Random r, string name, Gender gender, int regionId)
        {
            identity = new Identity(r, name, gender, regionId);

            //Creates an empty wallet.
            wallet = new Wallet();
            wallet.AddDollars(100000);
            wallet.AddBattlePoints(500);
            wallet.AddPokeMiles(500);

            bag = new TrainerBag();

            creationTime = DateTime.Now;

            //Creates a TimeSpan with 0h, 0m and 0s.
            playTime = new TimeSpan(0, 0, 0);

            //Creates a new pokémon team. The size is always six.
            pokemonTeam = new Pokémon[6];
            pokemonTeam[0] = new Pokémon(r, 0);
            pokemonTeam[0].SetEO(identity);
            pokemonTeam[1] = new Pokémon(r, 3);
            pokemonTeam[1].SetEO(identity);
            pokemonTeam[2] = new Pokémon(r, 6);
            pokemonTeam[2].SetEO(identity);
            pokemonTeam[3] = new Pokémon(r, 9);
            pokemonTeam[3].SetEO(identity);
            pokemonTeam[4] = new Pokémon(r, 442);
            pokemonTeam[4].SetEO(identity);

            //Creates the pokémon vault. The size is always the same.
            pokemonVault = new Pokémon[3][];
            for (int i = 0; i < 3; i += 1)
            {
                pokemonVault[i] = new Pokémon[30];
            }

            pokemonVault[0][0] = new Pokémon(r);
            pokemonVault[0][0].SetEO(identity);
            pokemonVault[0][1] = new Pokémon(r);
            pokemonVault[0][1].SetEO(identity);
            pokemonVault[0][2] = new Pokémon(r);
            pokemonVault[0][2].SetEO(identity);
            pokemonVault[0][3] = new Pokémon(r);
            pokemonVault[0][3].SetEO(identity);
            pokemonVault[0][4] = new Pokémon(r);
            pokemonVault[0][4].SetEO(identity);
            pokemonVault[0][5] = new Pokémon(r);
            pokemonVault[0][5].SetEO(identity);
            pokemonVault[0][6] = new Pokémon(r);
            pokemonVault[0][6].SetEO(identity);
            pokemonVault[0][7] = new Pokémon(r);
            pokemonVault[0][7].SetEO(identity);
            pokemonVault[0][8] = new Pokémon(r);
            pokemonVault[0][8].SetEO(identity);

            pokemonVault[1][0] = new Pokémon(r);
            pokemonVault[1][0].SetEO(identity);
            pokemonVault[1][1] = new Pokémon(r);
            pokemonVault[1][1].SetEO(identity);
            pokemonVault[1][2] = new Pokémon(r);
            pokemonVault[1][2].SetEO(identity);

            capturedPokemon = 17;

            sprite = GenerateSprite();
        }

        public Wallet GetWallet() { return wallet; }

        public TrainerBag GetBag() { return bag; }

        public string GetName() { return identity.GetName(); }

        public Gender GetGender() { return identity.GetGender(); }

        public string GetId() { return identity.GetId(); }

        public int GetPokeDollars() { return wallet.GetDollars(); }

        public int GetBattlePoints() { return wallet.GetBattlePoints(); }

        public int GetPokeMiles() { return wallet.GetPokeMiles(); }

        public TimeSpan GetPlayTime() { return playTime; }

        public Pokémon[] GetPokemonTeam() { return pokemonTeam; }

        public Pokémon GetPokemonInTeam(int index) { return pokemonTeam[index]; }

        public Pokémon GetPokemonInVault(int vault, int index) { return pokemonVault[vault][index]; }

        public int GetCapturedPokemon() { return capturedPokemon; }

        public string GetSprite() { return sprite; }

        public string GetPokemonTeamFrontSprite(int index)
        {
            //In case the trainer does not have said pokémon.
            if (pokemonTeam[index] == null)
                return string.Empty;

            //Returns the sprite of a specific pokémon in your team.
            return pokemonTeam[index].GetSpecie().GetFrontSprite();
        }

        public string GetPokemonVaultFrontSprite(int vault, int index, bool keepVoid)
        {
            //In case the trainer does not have said pokémon, we return the empty image
            //as borders do not work if there is not an element inside.
            if (pokemonVault[vault][index] == null)
            {
                if (!keepVoid)
                    return string.Empty;
                else
                    return "/Images/void.png";
            }

            //Returns the sprite of a specific pokémon in your vault.
            return pokemonVault[vault][index].GetSpecie().GetFrontSprite();
        }

        public string GetPokemonBackSprite(int index)
        {
            //In case the trainer does not have said pokémon.
            if (pokemonTeam[index] == null)
                return string.Empty;

            //Returns the sprite of a specific pokémon in your team.
            return pokemonTeam[index].GetSpecie().GetBackSprite();
        }

        public void AddPlayTime(TimeSpan playTime) { this.playTime += playTime; }

        public void SwapPokemonTeamPositions(int firstIndex, int secondIndex)
        {
            Pokémon aux = pokemonTeam[firstIndex];
            pokemonTeam[firstIndex] = pokemonTeam[secondIndex];
            pokemonTeam[secondIndex] = aux;
        }

        public void SwapPokemonVaultPositions(int firstVault, int secondVault, int firstIndex, int secondIndex)
        {
            Pokémon aux = pokemonVault[firstVault][firstIndex];
            pokemonVault[firstVault][firstIndex] = pokemonVault[secondVault][secondIndex];
            pokemonVault[secondVault][secondIndex] = aux;
        }

        public void SwapPokemonBetweenVaultAndTeam(int vault, int vaultIndex, int teamIndex)
        {
            Pokémon aux = pokemonVault[vault][vaultIndex];
            pokemonVault[vault][vaultIndex] = pokemonTeam[teamIndex];
            pokemonTeam[teamIndex] = aux;
        }

        public void ReleasePokemom(int vault, int index)
        {
            pokemonVault[vault][index] = null;
        }

        public void ReleasePokemom(int index)
        {
            pokemonTeam[index] = null;
        }

        private string GenerateSprite()
        {
            switch (identity.GetGender())
            {
                case Gender.MALE:
                    {
                        return "/Images/maleTrainer.png";
                    }
                case Gender.FEMALE:
                    {
                        return "/Images/femaleTrainer.png";
                    }
                case Gender.GENDERLESS:
                    {
                        return "/Images/otherTrainer.png";
                    }
                default:
                    {
                        //Not happening. But compiler.
                        return "/Images/otherTrainer.png";
                    }
            }
        }

        public override string ToString()
        {
            return
                "Name: " + GetName() + "\n" +
                "Gender: " + GetGender().ToString() + "\n" +
                "ID: " + GetId() + "\n" +
                "Money: " + GetPokeDollars() + "\n" +
                "Points: " + GetBattlePoints() + " BP / " + GetPokeMiles() + " PM\n" +
                "Creation date: " + creationTime.ToString("dd/MM/yyyy HH:mm:ss") + "\n" +
                "Play time: " + $"{playTime.Hours:00}:{playTime.Minutes:00}" + "\n" +
                "Pokémon: " + capturedPokemon.ToString();
        }
    }

    [Serializable]
    public class Identity
    {
        string name;
        Gender gender;
        string id;
        int secretNumber;

        public Identity(Random r, string name, Gender gender, int regionId)
        {
            //Personal data.
            this.name = name;
            this.gender = gender;
            id = GenerateID(r, regionId);
            secretNumber = GenerateSecretNumber(r);
        }

        public string GetName() { return name; }

        public Gender GetGender() { return gender; }

        public string GetId() { return id; }

        public int GetSecretNumber() { return secretNumber; }

        private string GenerateID(Random r, int regionId)
        {
            //Returns the ID (10 digits) with the specific region number (1d) and a random (9d).
            return regionId.ToString() + r.Next(100000000, 999999999).ToString();
        }

        private int GenerateSecretNumber(Random r)
        {
            //Return a 9 digit number.
            return r.Next(100000000, 999999999);
        }
    }

    [Serializable]
    public class Wallet
    {
        int pokeDollars;
        int battlePoints;
        int pokeMiles;

        public Wallet()
        {
            pokeDollars = 0;
            battlePoints = 0;
            pokeMiles = 0;
        }

        public int GetDollars() { return pokeDollars; }

        public int GetBattlePoints() { return battlePoints; }

        public int GetPokeMiles() { return pokeMiles; }

        public void AddDollars(int increase) { pokeDollars += increase; }

        public void AddBattlePoints(int increase) { battlePoints += increase; }

        public void AddPokeMiles(int increase) { pokeMiles += increase; }

        public void DeductDollars(int decrease) { pokeDollars -= decrease; }

        public void DeductBattlePoints(int decrease) { battlePoints -= decrease; }

        public void DeductPokeMiles(int decrease) { pokeMiles -= decrease; }
    }
}
