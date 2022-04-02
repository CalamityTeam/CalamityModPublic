using CalamityMod.World;
using Terraria.ModLoader;

namespace CalamityMod.CustomRecipes
{
    public class SchematicRecipe : ModRecipe
    {
        public string SchematicName;
        public SchematicRecipe(Mod mod, string schematicName) : base(mod) => SchematicName = schematicName;

        public override bool RecipeAvailable()
        {
            // Allow old worlds to craft the new schematics; They don't exist in old worlds
            if (!CalamityWorld.IsWorldAfterDraedonUpdate)
                return true;

            switch (SchematicName)
            {
                case "Sunken Sea":
                    return RecipeUnlockHandler.HasFoundSunkenSeaSchematic;
                case "Planetoid":
                    return RecipeUnlockHandler.HasFoundPlanetoidSchematic;
                case "Jungle":
                    return RecipeUnlockHandler.HasFoundJungleSchematic;
                case "Hell":
                    return RecipeUnlockHandler.HasFoundHellSchematic;
                case "Ice":
                    return RecipeUnlockHandler.HasFoundIceSchematic;
            }
            return false;
        }
    }
}
