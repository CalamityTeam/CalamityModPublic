using CalamityMod.World;
using Terraria.ModLoader;

namespace CalamityMod.CustomRecipes
{
    public class ArsenalTierGatedRecipe : ModRecipe
    {
        public int Tier;
        public bool AllowedInOldWorlds;
        public ArsenalTierGatedRecipe(Mod mod, int tier, bool allowedInOldWorlds = false) : base(mod)
        {
            Tier = tier;
            AllowedInOldWorlds = allowedInOldWorlds;
        }

        public static bool HasTierBeenLearned(int tier)
        {
            switch (tier)
            {
                case 1:
                    return RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes;
                case 2:
                    return RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes;
                case 3:
                    return RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes;
                case 4:
                    return RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes;
                case 5:
                    return RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes;
            }
            return false;
        }

        public override bool RecipeAvailable()
        {
            if (AllowedInOldWorlds && !CalamityWorld.IsWorldAfterDraedonUpdate)
                return true;

            switch (Tier)
            {
                case 1:
                    return RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes;
                case 2:
                    return RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes;
                case 3:
                    return RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes;
                case 4:
                    return RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes;
                case 5:
                    return RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes;
            }
            return false;
        }
    }
}