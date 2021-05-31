using Terraria.ModLoader;

namespace CalamityMod.CustomRecipes
{
    public class ArsenalTierGatedRecipe : ModRecipe
    {
        public int Tier;
        public ArsenalTierGatedRecipe(Mod mod, int tier) : base(mod) => Tier = tier;

        public override bool RecipeAvailable()
        {
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