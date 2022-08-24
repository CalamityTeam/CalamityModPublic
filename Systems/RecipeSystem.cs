using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class RecipeSystem : ModSystem
    {
        #region Late Loading
        public override void PostAddRecipes()
        {
            // This is placed here so that all tiles from all mods are guaranteed to be loaded at this point.
            TileFraming.Load();
        }
        #endregion

        #region Recipes
        public override void AddRecipeGroups() => CalamityRecipes.AddRecipeGroups();

        public override void AddRecipes() => CalamityRecipes.AddRecipes();
        #endregion
    }
}
