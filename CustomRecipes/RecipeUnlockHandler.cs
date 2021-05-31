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

        public static void Save(List<string> boolTagContainer)
        {
            boolTagContainer.AddWithCondition("HasUnlockedT1ArsenalRecipes", HasUnlockedT1ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT2ArsenalRecipes", HasUnlockedT2ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT3ArsenalRecipes", HasUnlockedT3ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT4ArsenalRecipes", HasUnlockedT4ArsenalRecipes);
            boolTagContainer.AddWithCondition("HasUnlockedT5ArsenalRecipes", HasUnlockedT5ArsenalRecipes);
        }

        public static void Load(IList<string> boolTagContainer)
        {
            HasUnlockedT1ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT1ArsenalRecipes");
            HasUnlockedT2ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT2ArsenalRecipes");
            HasUnlockedT3ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT3ArsenalRecipes");
            HasUnlockedT4ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT4ArsenalRecipes");
            HasUnlockedT5ArsenalRecipes = boolTagContainer.Contains("HasUnlockedT5ArsenalRecipes");
        }

        public static void SendData(BinaryWriter writer)
        {
            BitsByte flags = new BitsByte();
            flags[0] = HasUnlockedT1ArsenalRecipes;
            flags[1] = HasUnlockedT2ArsenalRecipes;
            flags[2] = HasUnlockedT3ArsenalRecipes;
            flags[3] = HasUnlockedT4ArsenalRecipes;
            flags[4] = HasUnlockedT5ArsenalRecipes;

            writer.Write(flags);
        }

        public static void ReceiveData(BinaryReader reader)
        {
            BitsByte flags = reader.ReadByte();
            HasUnlockedT1ArsenalRecipes = flags[0];
            HasUnlockedT2ArsenalRecipes = flags[1];
            HasUnlockedT3ArsenalRecipes = flags[2];
            HasUnlockedT4ArsenalRecipes = flags[3];
            HasUnlockedT5ArsenalRecipes = flags[4];
        }
    }
}