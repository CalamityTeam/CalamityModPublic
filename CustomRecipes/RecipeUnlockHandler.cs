using System.Collections.Generic;
using System.IO;
using Terraria;

namespace CalamityMod.CustomRecipes
{
    public static class RecipeUnlockHandler
    {
        public static bool HasUnlockedT1ArsenalRecipes = false;
        public static bool HasUnlockedT2ArsenalRecipes = false;
        public static bool HasUnlockedT3ArsenalRecipes = false;
        public static bool HasUnlockedT4ArsenalRecipes = false;
        public static bool HasUnlockedT5ArsenalRecipes = false;

        public static bool HasFoundSunkenSeaSchematic = false;
        public static bool HasFoundPlanetoidSchematic = false;
        public static bool HasFoundJungleSchematic = false;
        public static bool HasFoundHellSchematic = false;
        public static bool HasFoundIceSchematic = false;

        public static bool HasFoundLuxorsGift = false;
        public static bool HasFoundFungalSymbiote = false;
        public static bool HasFoundTrinketOfChi = false;
        public static bool HasFoundFrozenCube = false;
        public static bool HasFoundGladiatorsLocket = false;
        public static bool HasFoundUnstableGraniteCore = false;
        public static bool HasFoundCrimsonEffigy = false;
        public static bool HasFoundCorruptionEffigy = false;


        public static void Save(List<string> boolTagContainer)
        {
            boolTagContainer.AddWithCondition("HasUnlockedT1ArsenalRecipes", HasUnlockedT1ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT2ArsenalRecipes", HasUnlockedT2ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT3ArsenalRecipes", HasUnlockedT3ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT4ArsenalRecipes", HasUnlockedT4ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT5ArsenalRecipes", HasUnlockedT5ArsenalRecipes);

            boolTagContainer.AddWithCondition("HasFoundSunkenSeaSchematic", HasFoundSunkenSeaSchematic);
            boolTagContainer.AddWithCondition("HasFoundPlanetoidSchematic", HasFoundPlanetoidSchematic);
            boolTagContainer.AddWithCondition("HasFoundJungleSchematic", HasFoundJungleSchematic);
            boolTagContainer.AddWithCondition("HasFoundHellSchematic", HasFoundHellSchematic);
            boolTagContainer.AddWithCondition("HasFoundIceSchematic", HasFoundIceSchematic);

            boolTagContainer.AddWithCondition("HasFoundLuxorsGift", HasFoundLuxorsGift);
            boolTagContainer.AddWithCondition("HasFoundFungalSymbiote", HasFoundFungalSymbiote);
            boolTagContainer.AddWithCondition("HasFoundTrinketOfChi", HasFoundTrinketOfChi);
            boolTagContainer.AddWithCondition("HasFoundFrozenCube", HasFoundFrozenCube);
            boolTagContainer.AddWithCondition("HasFoundGladiatorsLocket", HasFoundGladiatorsLocket);
            boolTagContainer.AddWithCondition("HasFoundUnstableGraniteCore", HasFoundUnstableGraniteCore);
            boolTagContainer.AddWithCondition("HasFoundCrimsonEffigy", HasFoundCrimsonEffigy);
            boolTagContainer.AddWithCondition("HasFoundCorruptionEffigy", HasFoundCorruptionEffigy);
        }

        public static void Load(IList<string> boolTagContainer)
        {
            HasUnlockedT1ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT1ArsenalRecipes");
            HasUnlockedT2ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT2ArsenalRecipes");
            HasUnlockedT3ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT3ArsenalRecipes");
            HasUnlockedT4ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT4ArsenalRecipes");
            HasUnlockedT5ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT5ArsenalRecipes");

            HasFoundSunkenSeaSchematic = boolTagContainer.Contains("HasFoundSunkenSeaSchematic");
            HasFoundPlanetoidSchematic = boolTagContainer.Contains("HasFoundPlanetoidSchematic");
            HasFoundJungleSchematic = boolTagContainer.Contains("HasFoundJungleSchematic");
            HasFoundHellSchematic = boolTagContainer.Contains("HasFoundHellSchematic");
            HasFoundIceSchematic = boolTagContainer.Contains("HasFoundIceSchematic");

            HasFoundLuxorsGift = boolTagContainer.Contains("HasFoundLuxorsGift");
            HasFoundFungalSymbiote = boolTagContainer.Contains("HasFoundFungalSymbiote");
            HasFoundTrinketOfChi = boolTagContainer.Contains("HasFoundTrinketOfChi");
            HasFoundFrozenCube = boolTagContainer.Contains("HasFoundFrozenCube");
            HasFoundGladiatorsLocket = boolTagContainer.Contains("HasFoundGladiatorsLocket");
            HasFoundUnstableGraniteCore = boolTagContainer.Contains("HasFoundUnstableGraniteCore");
            HasFoundCrimsonEffigy = boolTagContainer.Contains("HasFoundCrimsonEffigy");
            HasFoundCorruptionEffigy = boolTagContainer.Contains("HasFoundCorruptionEffigy");

        }

        public static void SendData(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = HasUnlockedT1ArsenalRecipes;
            flags[1] = HasUnlockedT2ArsenalRecipes;
            flags[2] = HasUnlockedT3ArsenalRecipes;
            flags[3] = HasUnlockedT4ArsenalRecipes;
            flags[4] = HasUnlockedT5ArsenalRecipes;
            flags[5] = HasFoundSunkenSeaSchematic;
            flags[6] = HasFoundPlanetoidSchematic;
            flags[7] = HasFoundJungleSchematic;

            BitsByte flags2 = new BitsByte();
            flags2[0] = HasFoundHellSchematic;
            flags2[1] = HasFoundIceSchematic;
            flags2[2] = HasFoundLuxorsGift;
            flags2[3] = HasFoundFungalSymbiote;
            flags2[4] = HasFoundTrinketOfChi;
            flags2[5] = HasFoundGladiatorsLocket;
            flags2[6] = HasFoundUnstableGraniteCore;
            flags2[7] = HasFoundCrimsonEffigy;

            BitsByte flags3 = new BitsByte();
            flags3[0] = HasFoundCorruptionEffigy;

            writer.Write(flags);
            writer.Write(flags2);
            writer.Write(flags3);
        }

        public static void ReceiveData(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            HasUnlockedT1ArsenalRecipes = flags[0];
            HasUnlockedT2ArsenalRecipes = flags[1];
            HasUnlockedT3ArsenalRecipes = flags[2];
            HasUnlockedT4ArsenalRecipes = flags[3];
            HasUnlockedT5ArsenalRecipes = flags[4];
            HasFoundSunkenSeaSchematic = flags[5];
            HasFoundPlanetoidSchematic = flags[6];
            HasFoundJungleSchematic = flags[7];

            BitsByte flags2 = reader.ReadByte();
            HasFoundHellSchematic = flags2[0];
            HasFoundIceSchematic = flags2[1];
            HasFoundLuxorsGift = flags2[2];
            HasFoundFungalSymbiote = flags2[3];
            HasFoundTrinketOfChi = flags2[4];
            HasFoundGladiatorsLocket = flags2[5];
            HasFoundUnstableGraniteCore = flags2[6];
            HasFoundCrimsonEffigy = flags2[7];

            BitsByte flags3 = reader.ReadByte();
            HasFoundCorruptionEffigy = flags3[0];

        }
    }
}
