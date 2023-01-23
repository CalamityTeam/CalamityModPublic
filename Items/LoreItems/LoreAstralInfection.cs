using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAstralInfection")]
    public class LoreAstralInfection : LoreItem
    {
        public override string Lore =>
@"This twisted dreamscape is a starborne equivalent of the mundane rot you see in your lands.
I do not claim to understand the process in detail, but even the stars above can die. Left unchecked, their corpses bloat and fester.
Typically, some semblance of order is maintained. It is not unlike the circle of life.
Cosmic beings patrol the fathomless void and pick at the carrion, leaving clean bones.
The infection itself is a disturbance from deep space. It has a mind of its own, and projects its will upon life and land.
Those whose minds can grasp the true form of the universe, are largely immune.
They cannot be starstruck by a supposed higher truth, let alone one preached by a pustule.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Astral Infection");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstrumDeusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
