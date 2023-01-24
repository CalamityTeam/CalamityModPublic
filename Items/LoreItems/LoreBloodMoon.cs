using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBloodMoon")]
    public class LoreBloodMoon : LoreItem
    {
        public override string Lore =>
@"This malevolence is not the work of any God. Blood moons trace their origins to the dawn of history.
It is an occurrence equally sinister and banal. Everyone is acclimated to the shambling hordes of undead.
Organized societies are not threatened in the slightest. If anything, they welcome the opportunity to train green foot soldiers.
Those with fire in their veins may strike out on their own, to revel in the slaughter.
That is how I remember the sleepless nights from my younger days… Knee deep in corpses.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Blood Moon");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.BloodMoonStarter).
                AddIngredient(ItemID.SoulofNight, 3).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
