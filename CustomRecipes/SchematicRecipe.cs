using System;
using Terraria;
using Terraria.Localization;

namespace CalamityMod.CustomRecipes
{
    public static class SchematicRecipe
    {
        // TODO -- Use an enumeration instead of strings in the switch cases.
        public static NetworkText ConstructRecipeCondition(string schematicName, out Predicate<Recipe> condition)
        {
            switch (schematicName)
            {
                case "Sunken Sea":
                default:
                    condition = r => RecipeUnlockHandler.HasFoundSunkenSeaSchematic;
                    return NetworkText.FromKey($"Mods.CalamityMod.SunkenSeaSchematicRecipeCondition");
                case "Planetoid":
                    condition = r => RecipeUnlockHandler.HasFoundPlanetoidSchematic;
                    return NetworkText.FromKey($"Mods.CalamityMod.PlanetoidSchematicRecipeCondition");
                case "Jungle":
                    condition = r => RecipeUnlockHandler.HasFoundJungleSchematic;
                    return NetworkText.FromKey($"Mods.CalamityMod.JungleSchematicRecipeCondition");
                case "Hell":
                    condition = r => RecipeUnlockHandler.HasFoundHellSchematic;
                    return NetworkText.FromKey($"Mods.CalamityMod.UnderworldSchematicRecipeCondition");
                case "Ice":
                    condition = r => RecipeUnlockHandler.HasFoundIceSchematic;
                    return NetworkText.FromKey($"Mods.CalamityMod.IceSchematicRecipeCondition");
            }
        }
    }
}
