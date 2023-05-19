using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.CustomRecipes
{
    public static class ArsenalTierGatedRecipe
    {
        public static LocalizedText ConstructRecipeCondition(int tier, out Func<bool> condition)
        {
            condition = new Func<bool>(()=>HasTierBeenLearned(tier));
            return Language.GetOrRegister($"Mods.CalamityMod.Misc.Tier{tier}ArsenalRecipeCondition");
        }

        public static bool HasTierBeenLearned(int tier)
        {
            return tier switch
            {
                1 => RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes,
                2 => RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes,
                3 => RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes,
                4 => RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes,
                5 => RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes,
                _ => false,
            };
        }
    }
}
