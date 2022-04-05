using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.CustomRecipes
{
    public static class ArsenalTierGatedRecipe
    {
        public static NetworkText ConstructRecipeCondition(int tier, out Predicate<Recipe> condition)
        {
            condition = r => HasTierBeenLearned(tier);
            return NetworkText.FromKey($"Mods.Calamity.Tier{tier}ArsenalRecipeCondition");
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
